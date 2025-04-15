using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Xml.Linq;
using DG.Tweening;
using System.Collections;
using UnityEngine.EventSystems;

namespace TimelineSpecificityMod
{
    public static class Hooks
    {
        public static int StoredID = -1;
        public static int HoverID = -1;
        public static Vector2 Default;
        public static bool SetDefault;
        public static void FixShaderStencil(this Material self)
        {
            self.SetFloat("_StencilComp", 8);
            self.SetFloat("_Stencil", 0);
            self.SetFloat("_StencilOp", 0);
            self.SetFloat("_StencilWriteMask", 255);
            self.SetFloat("_StencilReadMask", 255);
            self.SetFloat("_ColorMask", 15);
        }
        public static void IconSpecify(this TimelineSlotLayout self, bool click)
        {
            /*if (!SetDefault)
            {
                Default = self._enemyIcon.rectTransform.sizeDelta;
                SetDefault = true;
            }
            if (click)
                self._enemyIcon.rectTransform.sizeDelta = new Vector2(Default.x * 1.5f, Default.y * 1.5f);
            else
                self._enemyIcon.rectTransform.sizeDelta = new Vector2(Default.x * 1.25f, Default.y * 1.25f);
            return;*/
            if (self._enemyIcon != null && !self._enemyIcon.Equals(null))
            {
                self._enemyIcon.material = new Material(LoadedDBsHandler.CombatData.EnemyMaterialTemplate);
                self._enemyIcon.material.FixShaderStencil();
                self._enemyIcon.materialForRendering.SetColor("_OutlineColor", click ? LoadedDBsHandler.CombatData.EnemyTurnColor : LoadedDBsHandler.CombatData.EnemyHoverColor);
                self._enemyIcon.materialForRendering.SetFloat("_OutlineAlpha", 1);
            }
        }
        public static void IconDespecify(this TimelineSlotLayout self)
        {
            /*if (!SetDefault)
            {
                Debug.LogWarning("Default Sizse Not Set!");
                return;
            }
            self._enemyIcon.rectTransform.sizeDelta = Default;
            return;*/
            if (self._enemyIcon != null && !self._enemyIcon.Equals(null))
            {
                /*if (self._enemyIcon.material != LoadedDBsHandler.CombatData.EnemyMaterialTemplate || !self._enemyIcon.material.Equals(LoadedDBsHandler.CombatData.EnemyMaterialTemplate))
                {
                    //Debug.Log("timelineselectmod: setting timeline sprite material");
                    self._enemyIcon.material = LoadedDBsHandler.CombatData.EnemyMaterialTemplate;
                }
                //Debug.Log("timelineselectmod: setting despesify colors");*/
                self._enemyIcon.materialForRendering.SetColor("_OutlineColor", LoadedDBsHandler.CombatData.EnemyBasicColor);
                self._enemyIcon.materialForRendering.SetFloat("_OutlineAlpha", 0);
            }
        }
        public static void HighlightFromEnemy(this EnemyInFieldLayout self, bool click)
        {
            HighlightFromID(self.EnemyID, click);
        }
        public static void DehighlightFromEnemy(this EnemyInFieldLayout self)
        {
            DehighlightFromID(self.EnemyID);
        }
        public static int GetIDFromtTimeline(this TimelineSlotLayout self)
        {
            List<TimelineInfo> timelineSlotInfo = CombatManager.Instance._stats.combatUI._TimelineHandler.TimelineSlotInfo;
            if (self.TimelineSlotID < 0 || self.TimelineSlotID >= timelineSlotInfo.Count) return -1;
            return timelineSlotInfo[self.TimelineSlotID].enemyID;
        }
        public static void HighlightFromTimeline(this TimelineSlotLayout self, bool click)
        {
            HighlightFromID(self.GetIDFromtTimeline(), click);
        }
        public static void DehighlightFromTimeline(this TimelineSlotLayout self)
        {
            DehighlightFromID(self.GetIDFromtTimeline());
        }
        public static void HighlightFromID(int self, bool click)
        {
            if (CombatManager.Instance._stats.timeline.IsConfused) return;
            for (int i = 0; i < CombatManager.Instance._stats.combatUI._TimelineHandler.TimelineSlotInfo.Count; i++)
            {
                TimelineInfo timeline = CombatManager.Instance._stats.combatUI._TimelineHandler.TimelineSlotInfo[i];
                if (timeline.enemyID == self)
                {
                    if (timeline.isSecret) continue;
                    foreach (TimelineSlotGroup slotgroup in CombatManager.Instance._stats.combatUI._timeline._slotsInUse)
                    {
                        if (slotgroup.slot.TimelineSlotID == i) slotgroup.slot.IconSpecify(click);
                    }
                }
            }
        }
        public static void DehighlightFromID(int self)
        {
            for (int i = 0; i < CombatManager.Instance._stats.combatUI._TimelineHandler.TimelineSlotInfo.Count; i++)
            {
                TimelineInfo timeline = CombatManager.Instance._stats.combatUI._TimelineHandler.TimelineSlotInfo[i];
                if (timeline.enemyID == self)
                {
                    foreach (TimelineSlotGroup slotgroup in CombatManager.Instance._stats.combatUI._timeline._slotsInUse)
                    {
                        if (slotgroup.slot.TimelineSlotID == i) slotgroup.slot.IconDespecify();
                    }
                }
            }
        }
        public static void CleanAllTimeline(bool onlyUnused = false)
        {
            foreach (TimelineSlotGroup slotgroup in CombatManager.Instance._stats.combatUI._timeline._unusedSlots)
            {
                slotgroup.slot.IconDespecify();
            }
            if (onlyUnused) return;
            foreach (TimelineSlotGroup slotgroup in CombatManager.Instance._stats.combatUI._timeline._slotsInUse)
            {
                slotgroup.slot.IconDespecify();
            }
        }

        //hooks

        public static void TimelineZoneLayout_UpdateTimelineContentSize(Action<TimelineZoneLayout, float> orig, TimelineZoneLayout self, float slots)
        {
            orig(self, slots);
            CleanAllTimeline(true);
        }
        public static IEnumerator TimelineZoneLayout_PopulateTimeline(Func<TimelineZoneLayout, TimelineInfo[], IEnumerator> orig, TimelineZoneLayout self, TimelineInfo[] timeline)
        {
            DehighlightFromID(StoredID);
            StoredID = -1;
            return orig(self, timeline);
        }

        public static void EnemyInFieldLayout_OnPointerEnter(Action<EnemyInFieldLayout, PointerEventData> orig, EnemyInFieldLayout self, PointerEventData eventData)
        {
            orig(self, eventData);
            if (StoredID != self.EnemyID) self.HighlightFromEnemy(false);
            HoverID = self.EnemyID;
        }
        public static void EnemyInFieldLayout_OnPointerExit(Action<EnemyInFieldLayout, PointerEventData> orig, EnemyInFieldLayout self, PointerEventData eventData)
        {
            orig(self, eventData);
            if (StoredID != self.EnemyID) self.DehighlightFromEnemy();
            HoverID = -1;
        }
        public static void EnemyInFieldLayout_OnPointerClick(Action<EnemyInFieldLayout, PointerEventData> orig, EnemyInFieldLayout self, PointerEventData eventData)
        {
            orig(self, eventData);
            DehighlightFromID(StoredID);
            StoredID = self.EnemyID;
            self.HighlightFromEnemy(true);
        }

        public static void TimelineSlotLayout_OnPointerEnter(Action<TimelineSlotLayout, PointerEventData> orig, TimelineSlotLayout self, PointerEventData eventData)
        {
            orig(self, eventData);
            if (self.GetIDFromtTimeline() != StoredID) self.HighlightFromTimeline(false);
            HoverID = self.GetIDFromtTimeline();
        }
        public static void TimelineSlotLayout_OnPointerExit(Action<TimelineSlotLayout, PointerEventData> orig, TimelineSlotLayout self, PointerEventData eventData)
        {
            orig(self, eventData);
            if (self.GetIDFromtTimeline() != StoredID) self.DehighlightFromTimeline();
            HoverID = -1;
        }
        public static void TimelineSlotLayout_OnPointerClick(Action<TimelineSlotLayout, PointerEventData> orig, TimelineSlotLayout self, PointerEventData eventData)
        {
            orig(self, eventData);
            DehighlightFromID(StoredID);
            self.HighlightFromTimeline(true);
            StoredID = self.GetIDFromtTimeline();
        }

        public static void CombatVisualizationController_TryHideEnemyIDInformation(Action<CombatVisualizationController, int> orig, CombatVisualizationController self, int id)
        {
            orig(self, id);
            if (id != HoverID)
            {
                DehighlightFromID(id);
                if (id == StoredID) StoredID = -1;
            }
        }
    }
}
