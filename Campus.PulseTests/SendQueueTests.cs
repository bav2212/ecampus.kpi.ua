﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Campus.Pulse;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Campus.Core;
using Campus.Core.Interfaces;
using Campus.Core.Common.Extensions;
using System.Collections.Concurrent;
using Campus.Core.EventsArgs;

namespace Campus.Pulse.Tests
{
    [TestClass()]
    public class SendQueueTests
    {
        [TestMethod()]
        public void AddTest()
        {
            EventHandler<BeatEventArgs> OnBeat = (sender, e) => { };
            try
            {
                var testObject = new SendQueue<IMessage>(ref OnBeat);
                testObject.Add(new Message(data: "test"), () => true);
                testObject.Add(new Message(data: "test"), () => true);
                testObject.OnAnyCondition += (pairs) =>
                {
                    Assert.IsFalse(pairs.Count() == 2);
                };

                int j = 1;
                for (int i = 0; i < 10000; i++)
                {
                    Console.WriteLine(i);
                    if (i == 100 * j)
                    {
                        j++;
                        OnBeat(this, new BeatEventArgs<Client>(this, null));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [TestMethod()]
        public void AddAsyncTest()
        {
            try
            {
                EventHandler<BeatEventArgs> OnBeat = (sender, e) => { };
                ConcurrentBag<IMessage> testList = new ConcurrentBag<IMessage>();
                var testObject = new SendQueue<IMessage>(ref OnBeat);
                testObject.OnAnyCondition += (pairs) =>
                {
                    IMessage message = null;
                    while (pairs.TryDequeue(out message))
                    {
                        if (testList.Contains(message))
                            Assert.Fail();
                        testList.Add(message);
                    }
                };


                Task.Run(() =>
                {
                    testObject.Add(new Message(data: "test1"), () => true, 1);
                    testObject.Add(new Message(data: "test3"), () => true, 3);
                });

                Task.Run(() =>
                {
                    testObject.Add(new Message(data: "test1"), () => true, 1);
                    testObject.Add(new Message(data: "test2"), () => true, 2);
                });

                Task.Run(() =>
                {
                    testObject.Add(new Message(data: "test1"), () => true, 1);
                    testObject.Add(new Message(data: "test4"), () => true, 4);
                });

                int j = 1;
                for (int i = 0; i < 10000; i++)
                {
                    Console.WriteLine(i);
                    if (i == 100 * j)
                    {
                        j++;
                        OnBeat(this, new BeatEventArgs(this));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
