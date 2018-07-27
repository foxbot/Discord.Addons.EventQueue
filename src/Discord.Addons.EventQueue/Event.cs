namespace Discord.Addons.EventQueue
{
    /// <summary>
    /// Event wraps an EventHandler
    /// </summary>
    public struct Event
    {
        /// <summary>
        /// Name represents the name of the EventHandler
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Argument contains the flattened arguments of the event handler.
        /// 
        /// This property will be one of <see cref="Unit"/>, <see cref="object"/>, or <see cref="ValueTuple"/>
        /// </summary>
        public object Argument { get; }

        internal Event(string name, object arg)
        {
            Name = name;
            Argument = arg;
        }
    }
}
