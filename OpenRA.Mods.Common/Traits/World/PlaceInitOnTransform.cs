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
using System.Linq;
using OpenRA.Primitives;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Traits
{
	[Desc("Adds PlaceBuildingInit to the transformed actor so it is treated as 'placed' when transforming into a building (e.g., MCV -> Construction Yard). Attach to the transforming actor.")]
	public sealed class PlaceInitOnTransformInfo : TraitInfo
	{
		[Desc("Only add PlaceBuildingInit when the IntoActor has BuildingInfo.")]
		public readonly bool OnlyWhenIntoBuilding = true;

		[Desc("Optional: restrict to these IntoActor names (case-insensitive). Leave empty to apply to any IntoActor.")]
		public readonly string[] OnlyIntoActors = Array.Empty<string>();

		public override object Create(ActorInitializer init) { return new PlaceInitOnTransform(this); }
	}

	public sealed class PlaceInitOnTransform : ITransformActorInitModifier
	{
		readonly PlaceInitOnTransformInfo info;

		public PlaceInitOnTransform(PlaceInitOnTransformInfo info)
		{
			this.info = info;
		}

		void ITransformActorInitModifier.ModifyTransformActorInit(Actor self, TypeDictionary init)
		{
			var transforms = self.TraitOrDefault<Transforms>();
			if (transforms == null)
				return;

			var into = transforms.Info.IntoActor;

			if (info.OnlyIntoActors != null && info.OnlyIntoActors.Length > 0 &&
				!info.OnlyIntoActors.Any(n => string.Equals(n, into, StringComparison.OrdinalIgnoreCase)))
				return;

			var rules = self.World.Map.Rules;
			if (!rules.Actors.TryGetValue(into, out var intoInfo))
				return;

			if (info.OnlyWhenIntoBuilding && intoInfo.TraitInfoOrDefault<BuildingInfo>() == null)
				return;

			// Mark the transformed actor as created by placement so CoTBuildingEmitter sends 'placed'.
			init.Add(new PlaceBuildingInit());
		}
	}
}
