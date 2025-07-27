using System;
using System.Collections.Generic;
using UnityEngine;

public class CsvManager : Singleton<CsvManager>
{
        public Dictionary<int, InvestigationData> InvestigationData = new();
}