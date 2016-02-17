using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HallsByra.Rx.Tests
{
    public class CacheLastFacts
    {
        [Fact]
        public async Task returns_all_elements()
        {
            // Given
            var src = new [] {1,2,3}.ToObservable().CacheLast();

            // When
            var result = await src.ToArray();

            // Then
            result.Should().Equal(1, 2, 3);            
        }

        [Fact]
        public async Task returns_the_current_element_when_awaited()
        {
            // Given
            var src = new Subject<int>();
            var behave = src.CacheLast();
            var s1 = behave.Subscribe(_ => {});
            src.OnNext(11);

            // When
            var current = await behave.Take(1);

            // Then
            current.Should().Be(11);
        }

        [Fact]
        public void returns_the_current_element_when_a_new_subscription_is_made()
        {
            // Given
            var src = new Subject<int>();
            var behave = src.CacheLast();
            var s1Elements = new List<int>();
            var s2Elements = new List<int>();

            // When
            var s1 = behave.Subscribe(s1Elements.Add);
            src.OnNext(1);
            var s2 = behave.Subscribe(s2Elements.Add);

            // Then
            s1Elements.Should().Equal(1);
            s2Elements.Should().Equal(1);
        }


        [Fact]
        public void returns_the_current_element_when_a_new_subscription_is_made_to_a_concatenated_observable()
        {
            // Given
            var src = new Subject<int>();
            var behave = Observable.Return(0).Concat(src).CacheLast();
            var s1Elements = new List<int>();
            var s2Elements = new List<int>();

            // When
            var s1 = behave.Subscribe(s1Elements.Add);
            src.OnNext(1);
            var s2 = behave.Subscribe(s2Elements.Add);

            // Then
            s1Elements.Should().Equal(0, 1);
            s2Elements.Should().Equal(1);
        }

        [Fact]
        public void restarts_subscription_when_current_subscriptions_detach_and_reattach()
        {
            // Given
            var src = new Subject<int>();
            var behave = Observable.Return(0).Concat(src).CacheLast();
            var s1Elements = new List<int>();
            var s2Elements = new List<int>();

            // When - two subscriptions are established.
            var s1 = behave.Subscribe(s1Elements.Add);
            src.OnNext(1);
            var s2 = behave.Subscribe(s2Elements.Add);

            // When - the subscriptions are reestablished
            s1.Dispose();
            s2.Dispose();
            s1 = behave.Subscribe(s1Elements.Add);
            src.OnNext(100);
            s2 = behave.Subscribe(s2Elements.Add);

            // Then
            s1Elements.Should().Equal(0, 1, 0, 100);
            s2Elements.Should().Equal(1, 100);
        }

    }
}
