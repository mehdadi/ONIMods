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

using KMod;
using PeterHan.PLib;
using Steamworks;
using System;
using System.IO;
using System.Reflection;

namespace PeterHan.ModUpdateDate {
	/// <summary>
	/// Extension methods for Mod Update Date.
	/// </summary>
	internal static class ExtensionMethods {
		/// <summary>
		/// The epoch time for Steam time stamps.
		/// </summary>
		private static readonly System.DateTime UNIX_EPOCH = new System.DateTime(1970, 1, 1,
			0, 0, 0, DateTimeKind.Utc);

		/// <summary>
		/// Finds a Steam mod by its ID (published file ID).
		/// </summary>
		/// <param name="manager">The mod manager.</param>
		/// <param name="id">The steam mod ID.</param>
		/// <returns>The mod, or null if it is not found.</returns>
		internal static Mod FindSteamMod(this Manager manager, ulong id) {
			Mod result = null;
			string idString = id.ToString();
			foreach (var mod in manager.mods) {
				var label = mod.label;
				// Steam mod with the same ID
				if (label.id == idString && label.distribution_platform == Label.
						DistributionPlatform.Steam) {
					result = mod;
					break;
				}
			}
			return result;
		}

		/// <summary>
		/// Gets the temporary download path for a mod.
		/// </summary>
		/// <param name="id">The Steam mod ID.</param>
		/// <returns>The path where its temporary download will be stored.</returns>
		internal static string GetDownloadPath(ulong id) {
			return Path.Combine(ModUpdateDatePatches.OurModPath ?? Path.GetDirectoryName(
				Assembly.GetExecutingAssembly().Location), id + ".zip");
		}

		/// <summary>
		/// Gets the last modified date of a mod's local files.
		/// </summary>
		/// <param name="mod">The mod to check.</param>
		/// <returns>The date and time of its last modification.</returns>
		internal static System.DateTime GetLocalLastModified(this Mod mod) {
			if (mod == null)
				throw new ArgumentNullException("mod");
			var label = mod.label;
			var result = System.DateTime.UtcNow;
			if (label.distribution_platform == Label.DistributionPlatform.Steam) {
				// 260 = MAX_PATH
				if (SteamUGC.GetItemInstallInfo(mod.GetSteamModID(), out _,
						out string _, 260U, out uint timestamp) && timestamp > 0U)
					result = timestamp.UnixEpochToDateTime();
				else
					PUtil.LogWarning("Unable to get Steam install information for " +
						label.title);
			} else
				try {
					// Get the last modified date of its install path :/
					result = File.GetLastWriteTimeUtc(Path.GetFullPath(label.install_path));
				} catch (IOException) {
					// Unable to determine the date, so use UtcNow...
					PUtil.LogWarning("I/O error when determining last modified date for " +
						label.title);
				}
			return result;
		}

		/// <summary>
		/// Gets the last modified date of a mod on Steam.
		/// </summary>
		/// <param name="id">The Steam mod ID to check.</param>
		/// <param name="when">The location where the last updated date will be stored.</param>
		/// <returns>true if the date was determined, or false if it is invalid.</returns>
		internal static bool GetGlobalLastModified(this PublishedFileId_t id,
				out System.DateTime when) {
			bool result = false;
			var steamMod = SteamUGCService.Instance?.FindMod(id);
			if (steamMod != null) {
				result = true;
				when = ((uint)steamMod.lastUpdateTime).UnixEpochToDateTime();
			} else
				// Mod was not found
				when = System.DateTime.UtcNow;
			return result;
		}

		/// <summary>
		/// Retrieves the Steam ID for a Steam mod. Only works on Steam mods!
		/// </summary>
		/// <param name="mod">The mod to check.</param>
		/// <returns>The Steam mod ID.</returns>
		internal static PublishedFileId_t GetSteamModID(this Mod mod) {
			string id = mod.label.id;
			// This should never fail
			if (!ulong.TryParse(id, out ulong idLong))
				throw new InvalidOperationException("Steam mod with invalid ID " + id);
			return new PublishedFileId_t(idLong);
		}

		/// <summary>
		/// Removes old downloaded copies of a mod.
		/// </summary>
		/// <param name="id">The Steam mod ID.</param>
		internal static void RemoveOldDownload(ulong id) {
			try {
				File.Delete(GetDownloadPath(id));
			} catch (IOException) {
				PUtil.LogWarning("Unable to clean temporary download for mod ID {0:D}".F(id));
			} catch (UnauthorizedAccessException) {
				PUtil.LogWarning("Unable to access temporary download for mod ID {0:D}".F(id));
			}
		}

		/// <summary>
		/// Converts a time from Steam (seconds since Unix epoch) to a C# DateTime.
		/// </summary>
		/// <param name="timeSeconds">The timestamp since the epoch.</param>
		/// <returns>The UTC date and time that it represents.</returns>
		internal static System.DateTime UnixEpochToDateTime(this uint timeSeconds) {
			return UNIX_EPOCH.AddSeconds(timeSeconds);
		}
	}
}
