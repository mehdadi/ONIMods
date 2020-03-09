﻿/*
 * Copyright 2020 Peter Han
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or
 * substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
 * BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace PeterHan.AirlockDoor {
	/// <summary>
	/// Strings used in Airlock Door.
	/// </summary>
	public static class AirlockDoorStrings {
		// Airlock Door
		public static LocString AIRLOCKDOOR_NAME = STRINGS.UI.FormatAsLink("True Airlock", AirlockDoorConfig.ID);
		public static LocString AIRLOCKDOOR_DESCRIPTION = "Sucking Duplicants that have nowhere to go into space through " +
			STRINGS.UI.FormatAsLink("Mechanized Airlocks", PressureDoorConfig.ID) +
			" is poor taste. Now they can suffocate quietly on the other side of an airlock.";
		public static LocString AIRLOCKDOOR_EFFECT = string.Concat("Blocks ",
			STRINGS.UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
			" and ",
			STRINGS.UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
			" flow, even while Duplicants are passing.\n\nWill not allow passage when no ",
			STRINGS.UI.FormatAsLink("Power", "POWER"),
			" is available.\n\n",
			STRINGS.UI.FormatAsLink("Critters", "CRITTERS"),
			" cannot pass through this door unless it is set to Open.");

		public static LocString AIRLOCKDOOR_LOGIC_OPEN = "Open/Close";
		public static LocString AIRLOCKDOOR_LOGIC_OPEN_ACTIVE = STRINGS.UI.FormatAsAutomationState(
			"Green Signal", STRINGS.UI.AutomationState.Active) + ": Unlock and set to automatic";
		public static LocString AIRLOCKDOOR_LOGIC_OPEN_INACTIVE = STRINGS.UI.FormatAsAutomationState(
			"Red Signal", STRINGS.UI.AutomationState.Standby) + ": Close and lock";
	}
}
