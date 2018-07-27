using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord.WebSocket;

namespace Discord.Addons.EventQueue
{
    /// <summary>
    /// EventQueue provides a queue of Discord events to be handled sequentially on your own time
    /// </summary>
    /// <typeparam name="T">The type of <see cref="BaseSocketClient"/>you will be listening to events from</typeparam>
    public class EventQueue<T>
        where T : BaseSocketClient
    {
        /// <summary>
        /// Events contains a queue of <see cref="Event"/> structures, each wrapping a Discord event
        /// </summary>
        public ConcurrentQueue<Event> Events { get; }

        private readonly T _client;
        private readonly EventInfo[] _events;
        private readonly Dictionary<string, Delegate> _eventMap;

        /// <summary>
        /// Create a new EventQueue with an instance of a client
        /// </summary>
        /// <param name="client"></param>
        public EventQueue(T client)
        {
            _client = client;
            _events = typeof(T).GetEvents();
            _eventMap = new Dictionary<string, Delegate>();

            Events = new ConcurrentQueue<Event>();
        }

        /// <summary>
        /// Register an event for listening
        /// </summary>
        /// <param name="eventName">
        /// The name of the event as it appears on <see cref="T"/>,
        /// best to use <c>nameof()</c> here
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Throws when an event of name <paramref name="eventName"/> is already registered
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when an event of name <paramref name="eventName"/> could not be found on <see cref="T"/>
        /// </exception>
        /// <exception cref="NotImplementedException">
        /// Throws if the event assosciated with <paramref name="eventName"/> has more than 3 arguments.
        /// </exception>
        /// <seealso cref="Unregister(string)"/>
        public void Register(string eventName)
        {
            if (_eventMap.ContainsKey(eventName))
                throw new InvalidOperationException($"The event '{eventName}' is already registered.");

            var ev = _events.FirstOrDefault(x => x.Name == eventName);
            if (ev == null)
                throw new ArgumentOutOfRangeException(nameof(eventName), "The event name is not a member of BaseSocketClient");

            Delegate handler = null;

            var argCount = ev.EventHandlerType.GenericTypeArguments.Count();
            switch (argCount)
            {
                case 1:
                    handler = (Func<Task>)(() => {
                        var e = new Event(eventName, new Unit());
                        Events.Enqueue(e);

                        return Task.CompletedTask;
                    });
                    break;
                case 2:
                    handler = (Func<object, Task>)((o1) => {
                        var e = new Event(eventName, o1);
                        Events.Enqueue(e);

                        return Task.CompletedTask;
                    });
                    break;
                case 3:
                    handler = (Func<object, object, Task>)((o1, o2) => {
                        var e = new Event(eventName, (o1, o2));
                        Events.Enqueue(e);

                        return Task.CompletedTask;
                    });
                    break;
                case 4:
                    handler = (Func<object, object, object, Task>)((o1, o2, o3) => {
                        var e = new Event(eventName, (o1, o2, o3));
                        Events.Enqueue(e);

                        return Task.CompletedTask;
                    });
                    break;
                default:
                    throw new NotImplementedException($"Unable to handle an event of arity/{argCount}");
            }

            ev.AddEventHandler(_client, handler);
            _eventMap.Add(eventName, handler);
        }

        /// <summary>
        /// Unregister an event that was already registered
        /// </summary>
        /// <param name="eventName">
        /// The name of the event as it appears on <see cref="T"/>,
        /// best to use <c>nameof()</c> here.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Throws when you try to unregister an event that is not registered.
        /// </exception>
        public void Unregister(string eventName)
        {
            if (!(_eventMap.TryGetValue(eventName, out var handler)))
                throw new InvalidOperationException($"The event '{eventName}' is not registered.");

            var ev = _events.FirstOrDefault(x => x.Name == eventName);
            ev.RemoveEventHandler(_client, handler);

            _eventMap.Remove(eventName);
        }
    }
}
