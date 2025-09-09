#region Copyright & License Information
/*
 * Copyright (c) The OpenRA Developers and Contributors
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using System;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.Logic
{
	public class ChainOfThoughtPromptLogic : ChromeLogic
	{
		// Increment when changing the content/behaviour of this prompt
		const int ChainOfThoughtVersion = 1;

		public static bool ShouldShowPrompt()
		{
			var ds = Game.Settings.Debug;
			return ds.ShowChainOfThoughtPrompt && ds.ChainOfThoughtPromptVersion < ChainOfThoughtVersion;
		}

		[ObjectCreator.UseCtor]
		public ChainOfThoughtPromptLogic(Widget widget, Action onComplete)
		{
			widget.Get<ButtonWidget>("CONTINUE_BUTTON").OnClick = () =>
			{
				Game.Settings.Debug.ChainOfThoughtPromptVersion = ChainOfThoughtVersion;
				Game.Settings.Save();
				Ui.CloseWindow();
				onComplete();
			};
		}
	}
}
