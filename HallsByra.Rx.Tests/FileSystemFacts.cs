using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HallsByra.Rx.Tests
{
    public class FileSystemFacts
    {
        public class WhenFileChanges
        {
            private string fileName = Path.Combine(Directory.GetCurrentDirectory(), "test.txt");

            [Fact]
            public async Task fires_once_when_a_file_is_changed()
            {
                // Given
                File.WriteAllText(fileName, "hubba");
                var results = new ReplaySubject<string>();
                FileSystem.WhenFileChanges(fileName).Subscribe(results);

                // When
                File.WriteAllText(fileName, "bubba");

                // Then
                var events = await results.TakeUntil(DateTime.Now + TimeSpan.FromMilliseconds(200)).ToArray();
                events.Should().HaveCount(1);
                events[0].Should().Be(fileName);
            }
        }
    }
}
