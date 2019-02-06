using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace HallsByra.Rx
{
    public static class IObservableExtensions
    {
        /// <summary>
        /// Silently skips null values from source.
        /// </summary>
        public static IObservable<T> SkipNull<T>(this IObservable<T> source) => source.Where(o => o != null);

        /// <summary>
        /// Combines each element with its predecessor into a Pair instance.
        /// </summary>
        /// <param name="firstPreviousValue">Value to use as Previous in the first produced pair.</param>
        public static IObservable<(T Previous, T Current)> PairWithPrevious<T>(this IObservable<T> source, T firstPreviousValue = default(T)) =>
              source.Scan(
                  (Previous: default(T), Current: firstPreviousValue),
                  (acc, current) => (Previous: acc.Current, Current: current));


        /// <summary>
        /// Multicast, with a ReplaySubject.
        /// </summary>
        public static IObservable<T> PublishReplay<T>(this IObservable<T> source, int bufferSize = 1) => source.Multicast(() => new ReplaySubject<T>(bufferSize));

        /// <summary>
        /// Multicast, with BehaviorSubject.
        /// </summary>
        public static IObservable<T> PublishBehavior<T>(this IObservable<T> source, Func<T> produceInitialValue) => source.Multicast(() => new BehaviorSubject<T>(produceInitialValue()));

        /// <summary>
        /// Variation of Multicast that accepts a subject factory instead of a subject instance. When the first subscription is
        /// made, the subject is created. When the last subscription is disposed, the subject is disposed. Each time the
        /// subscription count rises above zero, the subject is recreated.
        /// </summary>
        public static IObservable<T> Multicast<T>(this IObservable<T> source, Func<ISubject<T>> createSubject)
        {
            object mutex = new object();
            ISubject<T> subject = null;
            IDisposable subjectSubscription = null;
            int subscriptionCount = 0;

            return Observable.Create<T>(obs =>
            {
                lock (mutex)
                {
                    subscriptionCount++;
                    if (subject == null)
                    {
                        subject = createSubject();
                        subjectSubscription = source.Subscribe(subject);
                    }
                }

                var subscription = subject.Subscribe(obs);

                return () =>
                {
                    subscription.Dispose();
                    lock (mutex)
                    {
                        if (--subscriptionCount == 0)
                        {
                            subjectSubscription.Dispose();
                            subject = null;
                            subjectSubscription = null;
                        }
                    }
                };
            });

        }
    }
}
