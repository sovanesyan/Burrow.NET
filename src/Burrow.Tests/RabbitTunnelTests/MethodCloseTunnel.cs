﻿using System;
using Burrow.Tests.Extras.RabbitSetupTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using RabbitMQ.Client;

// ReSharper disable InconsistentNaming
namespace Burrow.Tests.RabbitTunnelTests
{
    [TestClass]
    public class MethodCloseTunnel
    {
        [TestMethod]
        public void Should_resubscribe_to_queues_after_reconnect_successfully()
        {
            // Arrange
            var newChannel = Substitute.For<IModel>();
            IDurableConnection durableConnection;
            var tunnel = RabbitTunnelForTest.CreateTunnel(newChannel, out durableConnection);
            tunnel.Subscribe<Customer>("subscriptionName", x => { });
            tunnel.Subscribe<Customer>("subscriptionName2", x => { });

            // Action
            durableConnection.Disconnected += Raise.Event<Action>();
            durableConnection.Connected += Raise.Event<Action>();

            // Assert
            newChannel.Received(4).BasicConsume("Queue", false, Arg.Is<string>(x => x.StartsWith("subscriptionName")), Arg.Any<IBasicConsumer>());
        }

        
    }
}
// ReSharper restore InconsistentNaming