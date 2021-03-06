﻿using Org.Kevoree.Core.Api;
using System;

namespace Org.Kevoree.Core
{
    public class TupleLockCallBack
    {
        private readonly LockCallBack callback;

        private readonly Guid guid;

        public TupleLockCallBack(LockCallBack callback, Guid guid)
        {
            this.callback = callback;
            this.guid = guid;
        }

        public LockCallBack getCallback()
        {
            return this.callback;
        }

        public Guid getGuid()
        {
            return this.guid;
        }


    }
}
