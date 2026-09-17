using Jotunn.Entities;
using Byname.Titles;

namespace Byname.Commands
{
    /// <summary>
    /// Answers "why am I called this".
    ///
    /// One registration serves both surfaces. Chat.InputText strips the leading '/' and
    /// hands the rest to Terminal.TryRunCommand (Chat.cs:456), which looks the name up in
    /// the same dictionary the F5 console uses — so this responds to <c>/byname</c> typed
    /// in chat and to <c>byname</c> typed in the console, with no Harmony patch at all.
    ///
    /// The Terminal passed to Run is whichever window invoked it, so the answer is written
    /// back where the player asked rather than always to one of them.
    /// </summary>
    internal sealed class BynameCommand : ConsoleCommand
    {
        public override string Name => "byname";

        public override string Help =>
            "Explain your current title and what earned it.";

        /// <summary>
        /// Not a cheat, so it works without devcommands. It only reads the player's own
        /// stats and changes nothing.
        /// </summary>
        public override bool IsCheat => false;

        public override void Run(string[] args, Terminal context)
        {
            if (context == null) return;

            try
            {
                foreach (var line in TitleService.Explain(Player.m_localPlayer))
                {
                    context.AddString(line);
                }
            }
            catch (System.Exception e)
            {
                // Never let a chat command throw into the terminal's own error handling.
                context.AddString("Byname could not explain your title; see the log.");
                BynamePlugin.LogError($"/byname failed: {e}");
            }
        }
    }
}
