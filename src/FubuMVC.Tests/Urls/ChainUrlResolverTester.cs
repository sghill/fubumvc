﻿using System;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Urls
{
	[TestFixture]
	public class ChainUrlResolverTester
	{
		private StandInHttpRequest theHttpRequest;
		private ChainUrlResolver theUrlResolver;

		private BehaviorGraph theGraph;
		private BehaviorChain theSimpleChain;
		private BehaviorChain theChainWithRouteParams;

		[SetUp]
		public void SetUp()
		{
			theHttpRequest = new StandInHttpRequest();
			UrlContext.Stub("http://server");

			theUrlResolver = new ChainUrlResolver(theHttpRequest);

			theGraph = BehaviorGraph.BuildFrom(registry =>
			{
				registry.Actions.IncludeType<ChainUrlResolverEndpoint>();
			});

			theSimpleChain = theGraph.BehaviorFor<ChainUrlResolverEndpoint>(x => x.get_index());
			theChainWithRouteParams = theGraph.BehaviorFor(typeof(ChainUrlParams));
		}

		[TearDown]
		public void TearDown()
		{
			UrlContext.Reset();
		}

		[Test]
		public void resolve_a_url_without_route_params()
		{
			theUrlResolver.UrlFor(null, theSimpleChain).ShouldEqual("http://server/index");
		}

		[Test]
		public void resolve_a_url_with_route_params_filled_by_input()
		{
			var input = new ChainUrlParams {Name = "Joel"};
			theUrlResolver.UrlFor(input, theChainWithRouteParams).ShouldEqual("http://server/Joel");
		}

		[Test]
		public void resolve_a_url_with_route_params_explicitly_filled()
		{
			var values = new RouteParameters();
			values["Name"] = "Joel";

			theUrlResolver.UrlFor(theChainWithRouteParams, values).ShouldEqual("http://server/Joel");
		}

		public class ChainUrlResolverEndpoint
		{
			public string get_Name(ChainUrlParams values)
			{
				throw new NotImplementedException();
			}

			public string get_index()
			{
				throw new NotImplementedException();
			}
		}

		public class ChainUrlParams
		{
			public string Name { get; set; }
		}
	}
}