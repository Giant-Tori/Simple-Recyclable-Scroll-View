using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tori.UI
{
    public struct TestSlotData : IRecyclableSlotData
    {
        public string name;
    }

    public class TestSlot : MonoBehaviour, IRecyclableSlot
    {
        [SerializeField] private TMP_Text _text;
        public void MakeSlot(IRecyclableSlotData slotData)
        {
            TestSlotData data = (TestSlotData)slotData;
            _text.text = data.name;
        }
    }
}