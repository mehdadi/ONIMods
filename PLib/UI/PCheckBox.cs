﻿/*
 * Copyright 2019 Peter Han
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

using System;
using UnityEngine;
using UnityEngine.UI;

namespace PeterHan.PLib.UI {
	/// <summary>
	/// A custom UI button check box class.
	/// </summary>
	public class PCheckBox : PTextComponent {
		/// <summary>
		/// The unchecked state.
		/// </summary>
		public const int STATE_UNCHECKED = 0;

		/// <summary>
		/// The checked state.
		/// </summary>
		public const int STATE_CHECKED = 1;

		/// <summary>
		/// The partially checked state.
		/// </summary>
		public const int STATE_PARTIAL = 2;

		/// <summary>
		/// Generates the checkbox image states.
		/// </summary>
		/// <param name="imageColor">The color style for the checked icon.</param>
		/// <param name="borderImage">The border image.</param>
		/// <returns>The states for this checkbox.</returns>
		private static ToggleState[] GenerateStates(ColorStyleSetting imageColor,
				Image borderImage) {
			var sps = new StatePresentationSetting() {
				color = imageColor.activeColor, use_color_on_hover = true,
				color_on_hover = imageColor.hoverColor, image_target = null,
				name = "Partial"
			};
			return new ToggleState[] {
				new ToggleState() {
					// Unchecked
					color = PUITuning.Colors.Transparent, color_on_hover = PUITuning.Colors.
					Transparent, sprite = null, use_color_on_hover = false,
					additional_display_settings = new StatePresentationSetting[] {
						new StatePresentationSetting() {
							color = imageColor.activeColor, use_color_on_hover = false,
							image_target = null, name = "Unchecked"
						}
					}
				},
				new ToggleState() {
					// Checked
					color = imageColor.activeColor, color_on_hover = imageColor.hoverColor,
					sprite = PUITuning.Images.Checked, use_color_on_hover = true,
					additional_display_settings = new StatePresentationSetting[] { sps }
				},
				new ToggleState() {
					// Partial
					color = imageColor.activeColor, color_on_hover = imageColor.hoverColor,
					sprite = PUITuning.Images.Partial, use_color_on_hover = true,
					additional_display_settings = new StatePresentationSetting[] { sps }
				}
			};
		}

		/// <summary>
		/// The check box color.
		/// </summary>
		public ColorStyleSetting CheckColor { get; set; }

		/// <summary>
		/// The check box's background color.
		/// </summary>
		public Color BackColor { get; set; }

		/// <summary>
		/// The size to scale the check box. If 0x0, it will not be scaled.
		/// </summary>
		public Vector2 CheckSize { get; set; }

		/// <summary>
		/// The initial check box state.
		/// </summary>
		public int InitialState { get; set; }

		/// <summary>
		/// The margin around the component.
		/// </summary>
		public RectOffset Margin { get; set; }

		/// <summary>
		/// The action to trigger on click. It is passed the realized source object.
		/// </summary>
		public PUIDelegates.OnChecked OnChecked;

		public PCheckBox() : this(null) { }

		public PCheckBox(string name) : base(name ?? "CheckBox") {
			BackColor = PUITuning.Colors.BackgroundLight;
			CheckColor = null;
			CheckSize = new Vector2(20.0f, 20.0f);
			IconSpacing = 3;
			InitialState = STATE_UNCHECKED;
			Margin = new RectOffset();
			Sprite = null;
			Text = null;
			ToolTip = "";
			WordWrap = false;
		}

		public override GameObject Build() {
			var checkbox = PUIElements.CreateUI(Name);
			// Background
			var trueColor = CheckColor ?? PUITuning.Colors.CheckboxWhiteStyle;
			// Checkbox background
			var checkBack = PUIElements.CreateUI("CheckBox");
			checkBack.AddComponent<Image>().color = BackColor;
			PUIElements.SetParent(checkBack, checkbox);
			// Checkbox border
			var checkBorder = PUIElements.CreateUI("CheckBorder");
			var borderImg = checkBorder.AddComponent<Image>();
			borderImg.sprite = PUITuning.Images.CheckBorder;
			borderImg.color = trueColor.activeColor;
			borderImg.type = Image.Type.Sliced;
			PUIElements.SetParent(checkBorder, checkBack);
			// Checkbox foreground
			var imageChild = PUIElements.CreateUI("CheckMark");
			var img = imageChild.AddComponent<Image>();
			PUIElements.SetParent(imageChild, checkBorder);
			img.sprite = PUITuning.Images.Checked;
			img.preserveAspect = true;
			// Limit size if needed
			if (CheckSize.x > 0.0f && CheckSize.y > 0.0f)
				PUIElements.SetSizeImmediate(imageChild, CheckSize);
			else
				PUIElements.AddSizeFitter(imageChild, false);
			BoxLayoutGroup.LayoutNow(checkBorder);
			BoxLayoutGroup.LayoutNow(checkBack);
			// Add foreground image since the background already has one
			if (Sprite != null)
				PLabel.ImageChildHelper(checkbox, Sprite, SpriteSize);
			// Add text
			if (!string.IsNullOrEmpty(Text))
				PLabel.TextChildHelper(checkbox, FontSize, Text, WordWrap);
			// Icon and text are side by side
			var lg = checkbox.AddComponent<HorizontalLayoutGroup>();
			lg.childAlignment = TextAnchor.MiddleLeft;
			lg.spacing = Math.Max(IconSpacing, 0);
			lg.padding = Margin;
			// Add tooltip
			if (!string.IsNullOrEmpty(ToolTip))
				checkbox.AddComponent<ToolTip>().toolTip = ToolTip;
			// Toggle
			var kButton = checkbox.AddComponent<MultiToggle>();
			var evt = OnChecked;
			if (evt != null)
				kButton.onClick += () => {
					evt?.Invoke(checkbox, kButton.CurrentState);
				};
			kButton.play_sound_on_click = true;
			kButton.play_sound_on_release = false;
			kButton.states = GenerateStates(trueColor, borderImg);
			kButton.toggle_image = img;
			kButton.ChangeState(InitialState);
			PUIElements.AddSizeFitter(checkbox, DynamicSize).SetFlexUISize(FlexSize).SetActive(
				true);
			InvokeRealize(checkbox);
			return checkbox;
		}

		/// <summary>
		/// Sets the default Klei pink button style as this button's color and text style.
		/// </summary>
		/// <returns>This button for call chaining.</returns>
		public PCheckBox SetKleiPinkStyle() {
			TextColor = PUITuning.UITextLightStyle;
			BackColor = PUITuning.Colors.ButtonPinkStyle.inactiveColor;
			CheckColor = PUITuning.Colors.CheckboxDarkStyle;
			return this;
		}

		/// <summary>
		/// Sets the default Klei blue button style as this button's color and text style.
		/// </summary>
		/// <returns>This button for call chaining.</returns>
		public PCheckBox SetKleiBlueStyle() {
			TextColor = PUITuning.UITextLightStyle;
			BackColor = PUITuning.Colors.ButtonBlueStyle.inactiveColor;
			CheckColor = PUITuning.Colors.CheckboxDarkStyle;
			return this;
		}
	}
}
