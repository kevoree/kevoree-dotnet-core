using Org.Kevoree.Core.Api.Handler;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Org.Kevoree.Core
{
    class KevoreeListeners
    {
        private KevoreeCoreBean kevoreeCoreBean;

        private readonly BlockingCollection<Action> scheduler = new BlockingCollection<Action>(new ConcurrentQueue<Action>());
        private Boolean schedulerStop = false;
        private readonly BlockingCollection<Action> schedulerAsync = new BlockingCollection<Action>(new ConcurrentQueue<Action>());
        private Boolean schedulerAsyncStop = false;
        private readonly List<ModelListener> registeredListeners = new List<ModelListener>();


        public KevoreeListeners(Org.Kevoree.Core.KevoreeCoreBean kevoreeCoreBean)
        {
            // TODO: Complete member initialization
            this.kevoreeCoreBean = kevoreeCoreBean;

            new Thread(new ThreadStart(() =>
            {
                while (!schedulerStop)
                {
                    var value = scheduler.Take();
                    Task.Run(value);
                }
            })).Start();

            new Thread(new ThreadStart(() =>
            {
                while (!schedulerAsyncStop)
                {
                    var value = schedulerAsync.Take();
                    Task.Run(value);
                }
            })).Start();

        }

        public void addListener(ModelListener l)
        {
            scheduler.Add(() => { if (!registeredListeners.Contains(l)) { registeredListeners.Add(l); } });
        }

        public void removeListener(ModelListener l)
        {
            scheduler.Add(() => { if (registeredListeners.Contains(l)) { registeredListeners.Remove(l); } });
        }

        public void notifyAllListener()
        {
            scheduler.Add(() =>
            {
                foreach (var ml in registeredListeners)
                {
                    schedulerAsync.Add(() =>
                    {
                        ml.modelUpdated();
                    });
                }
            });
        }

        public void stop()
        {
            registeredListeners.Clear();
            this.schedulerStop = true;
            this.schedulerAsyncStop = true;

        }

        internal bool preUpdate(Api.Handler.UpdateContext updateContext)
        {
            scheduler.Add(() => { foreach (var l in registeredListeners) { l.preUpdate(updateContext); } });

            // TODO : is this really necessary ?
            return true;
        }

        internal bool initUpdate(Api.Handler.UpdateContext updateContext)
        {
            scheduler.Add(() => { foreach (var l in registeredListeners) { l.initUpdate(updateContext); } });

            // TODO : is this really necessary ?
            return true;
        }


        public bool afterUpdate(UpdateContext updateContext)
        {
            scheduler.Add(() => { foreach (var l in registeredListeners) { l.afterLocalUpdate(updateContext); } });

            // TODO : is this really necessary ?
            return true;

        }

        public bool preRollback(UpdateContext updateContext)
        {
            scheduler.Add(() => { foreach (var l in registeredListeners) { l.preRollback(updateContext); } });

            // TODO : is this really necessary ?
            return true;

        }

        public bool postRollback(UpdateContext updateContext)
        {
            scheduler.Add(() => { foreach (var l in registeredListeners) { l.postRollback(updateContext); } });

            // TODO : is this really necessary ?
            return true;

        }
    }
}
