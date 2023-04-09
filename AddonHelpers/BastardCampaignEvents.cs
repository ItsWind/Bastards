using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace BastardChildren.AddonHelpers {
    public static class BastardCampaignEvents {
        // BASTARD CONCEPTION EVENT
        private static List<Action<Hero>> _onBastardConceptionAttemptActions = new();
        internal static void Fire_OnPlayerBastardConceptionAttempt(Hero other) {
            foreach (Action<Hero> action in _onBastardConceptionAttemptActions)
                action(other);
        }
        /// <summary>
        /// Adds a method to be run when a bastard conception is attempted.
        /// </summary>
        /// <param name="action">Action takes two parameters. Hero is for the other hero engaged in conception. bool is for if the act was cruel or not.</param>
        public static void AddAction_OnPlayerBastardConceptionAttempt(Action<Hero> action) {
            _onBastardConceptionAttemptActions.Add(action);
        }

        // PLAYER BECOMES BASTARD EVENT
        private static List<Action<Hero>> _onPlayerBecomeBastardActions = new();
        internal static void Fire_OnPlayerBecomeBastard(Hero newParent) {
            foreach (Action<Hero> action in _onPlayerBecomeBastardActions)
                action(newParent);
        }
        /// <summary>
        /// Adds a method to be run when the player becomes a bastard.
        /// </summary>
        /// <param name="action">Action takes one parameter. Hero is for the new parent of the player.</param>
        public static void AddAction_OnPlayerBecomeBastard(Action<Hero> action) {
            _onPlayerBecomeBastardActions.Add(action);
        }
    }
}
