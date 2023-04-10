using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace BastardChildren.AddonHelpers {
    public static class BastardCampaignEvents {
        // AI BASTARD CONCEPTION EVENT
        private static List<Action<Hero, Hero>> _onAIBastardConceptionAttemptActions = new();
        internal static void Fire_OnAIBastardConceptionAttempt(Hero father, Hero mother) {
            foreach (Action<Hero, Hero> action in _onAIBastardConceptionAttemptActions)
                action(father, mother);
        }
        /// <summary>
        /// Adds a method to be run when an AI bastard conception is attempted.
        /// </summary>
        /// <param name="action">Action takes two parameters. First Hero is the father, and the second is the mother.</param>
        public static void AddAction_OnAIBastardConceptionAttempt(Action<Hero, Hero> action) {
            _onAIBastardConceptionAttemptActions.Add(action);
        }

        // PLAYER BASTARD CONCEPTION EVENT
        private static List<Action<Hero>> _onPlayerBastardConceptionAttemptActions = new();
        internal static void Fire_OnPlayerBastardConceptionAttempt(Hero other) {
            foreach (Action<Hero> action in _onPlayerBastardConceptionAttemptActions)
                action(other);
        }
        /// <summary>
        /// Adds a method to be run when a player bastard conception is attempted through dialog.
        /// </summary>
        /// <param name="action">Action takes one parameter. Hero is for the other hero engaged in conception.</param>
        public static void AddAction_OnPlayerBastardConceptionAttempt(Action<Hero> action) {
            _onPlayerBastardConceptionAttemptActions.Add(action);
        }

        // PLAYER BECOMES BASTARD EVENT
        private static List<Action<Hero>> _onPlayerBecomeBastardActions = new();
        internal static void Fire_OnPlayerBecomeBastard(Hero newParent) {
            foreach (Action<Hero> action in _onPlayerBecomeBastardActions)
                action(newParent);
        }
        /// <summary>
        /// Adds a method to be run when the player becomes a bastard through dialog.
        /// </summary>
        /// <param name="action">Action takes one parameter. Hero is for the new parent of the player.</param>
        public static void AddAction_OnPlayerBecomeBastard(Action<Hero> action) {
            _onPlayerBecomeBastardActions.Add(action);
        }
    }
}
