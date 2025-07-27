using UnityEngine;
using System.Collections.Generic;

public class CsvManager : Singleton<CsvManager>
{
        public Dictionary<int, InvestigationData> InvestigationData = new();
}