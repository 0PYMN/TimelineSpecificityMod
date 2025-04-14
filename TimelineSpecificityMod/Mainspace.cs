using BepInEx;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TimelineSpecificityMod
{
    [BepInPlugin("000.TimelineSpecificity", "Timeline Icon Enemy Specificity Mod", "0.0.0")]
    public class Mainspace : BaseUnityPlugin
    {
        public void Awake() => Add();

        public static void Add()
        {
            new Hook(typeof(TimelineZoneLayout).GetMethod(nameof(TimelineZoneLayout.UpdateTimelineContentSize), ~BindingFlags.Default), typeof(Hooks).GetMethod(nameof(Hooks.TimelineZoneLayout_UpdateTimelineContentSize), ~BindingFlags.Default));
            new Hook(typeof(TimelineZoneLayout).GetMethod(nameof(TimelineZoneLayout.PopulateTimeline), ~BindingFlags.Default), typeof(Hooks).GetMethod(nameof(Hooks.TimelineZoneLayout_PopulateTimeline), ~BindingFlags.Default));

            new Hook(typeof(EnemyInFieldLayout).GetMethod(nameof(EnemyInFieldLayout.OnPointerEnter), ~BindingFlags.Default), typeof(Hooks).GetMethod(nameof(Hooks.EnemyInFieldLayout_OnPointerEnter), ~BindingFlags.Default));
            new Hook(typeof(EnemyInFieldLayout).GetMethod(nameof(EnemyInFieldLayout.OnPointerExit), ~BindingFlags.Default), typeof(Hooks).GetMethod(nameof(Hooks.EnemyInFieldLayout_OnPointerExit), ~BindingFlags.Default));
            new Hook(typeof(EnemyInFieldLayout).GetMethod(nameof(EnemyInFieldLayout.OnPointerClick), ~BindingFlags.Default), typeof(Hooks).GetMethod(nameof(Hooks.EnemyInFieldLayout_OnPointerClick), ~BindingFlags.Default));

            new Hook(typeof(TimelineSlotLayout).GetMethod(nameof(TimelineSlotLayout.OnPointerClick), ~BindingFlags.Default), typeof(Hooks).GetMethod(nameof(Hooks.TimelineSlotLayout_OnPointerEnter), ~BindingFlags.Default));
            new Hook(typeof(TimelineSlotLayout).GetMethod(nameof(TimelineSlotLayout.OnPointerExit), ~BindingFlags.Default), typeof(Hooks).GetMethod(nameof(Hooks.TimelineSlotLayout_OnPointerExit), ~BindingFlags.Default));
            new Hook(typeof(TimelineSlotLayout).GetMethod(nameof(TimelineSlotLayout.OnPointerClick), ~BindingFlags.Default), typeof(Hooks).GetMethod(nameof(Hooks.TimelineSlotLayout_OnPointerClick), ~BindingFlags.Default));

            new Hook(typeof(CombatVisualizationController).GetMethod(nameof(CombatVisualizationController.TryHideEnemyIDInformation), ~BindingFlags.Default), typeof(Hooks).GetMethod(nameof(Hooks.CombatVisualizationController_TryHideEnemyIDInformation), ~BindingFlags.Default));
        }
    }
}
