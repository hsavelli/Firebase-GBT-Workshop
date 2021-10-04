namespace Hamster.States
{
    // Utility state, for fetching strings.  Is basically
    // just a specialization of WaitingForDBLoad, but it needs
    // some minor changes to how the results are parsed, since
    // they're not technically valid json.
    class WaitingForDBString : WaitingForDBLoad<string>
    {

        public WaitingForDBString(string path) : base(path) { }

        protected override string ParseResult(string json)
        {
            return json.Length > 2 ? json.Substring(1, json.Length - 2) : null;
        }
    }
}