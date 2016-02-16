using FluentAssertions;
using HallsByra.Rx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HallsByra.Rx.Tests
{
    public class PairWithPreviousFacts
    {
        [Fact]
        public async Task returns_default_as_previous_for_the_first_element()
        {
            // Given
            var nums = new [] { "A", "B" }.ToObservable().PairWithPrevious();

            // Then
            var first = await nums.Take(1);
            first.Previous.Should().BeNull();
            first.Current.Should().Be("A");
        }

        [Fact]
        public async Task returns_provided_default_as_previous_for_the_first_element()
        {
            // Given
            var nums = new [] { "A", "B" }.ToObservable().PairWithPrevious("First");

            // Then
            var first = await nums.Take(1);
            first.Previous.Should().Be("First");
            first.Current.Should().Be("A");
        }

        [Fact]
        public async Task returns_the_previous_element_for_all_but_the_first()
        {
            // Given
            var nums = new [] { "A", "B" }.ToObservable().PairWithPrevious();

            // Then
            var result = await nums.ToArray();
            result.Should().HaveCount(2);
            result[0].Previous.Should().BeNull();
            result[0].Current.Should().Be("A");
            result[1].Previous.Should().Be("A");
            result[1].Current.Should().Be("B");
        }
    }
}
