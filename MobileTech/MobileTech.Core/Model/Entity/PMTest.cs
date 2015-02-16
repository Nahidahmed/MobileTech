using System;
using System.Xml.Serialization;

namespace MobileTech.Models.Entity
{
    #region Procedure

    public class Procedure {
        [XmlElement]
        public long PmpPrimaryId { get; set; }
        [XmlElement]
        public int Number { get; set; }
        [XmlElement(IsNullable = true)]
        public string Description { get; set; }
    }

    public class ProcedureInfo : Procedure {
        [XmlElement(IsNullable = true)]
        public string Type { get; set; }
        [XmlElement]
        public DateTime RevisionDate { get; set; }
        [XmlElement(IsNullable = true)]
        public string CheckList { get; set; }
    }

    public class ProcedureDetails : ProcedureInfo {
        [XmlElement(IsNullable = true)]
        public string Notes { get; set; }
        [XmlElement(IsNullable = true)]
        public string Text { get; set; }
        [XmlElement(IsNullable = true)]
        public PMTESTSProcedureStep[] ProcedureSteps { get; set; }
        [XmlElement(IsNullable = true)]
        public string WorkOrderTemplate { get; set; }
        [XmlElement]
        public decimal LaborTime { get; set; }
        [XmlElement(IsNullable = true)]
        public SystemCode ServiceCenter { get; set; }
        [XmlElement(IsNullable = true)]
        public MedTesterChecklistItem[] MedTesterItems { get; set; }
        [XmlElement(IsNullable = true)]
        public ProcedureChecklistTaskInfo[] ProcedureCheckListTasks { get; set; }
    }

    public enum SortProceduresBy {
        Attachments = 0,
        Type = 1,
        Procedure = 2,
        Description = 3,
        RevisionDate = 4,
        CheckList = 5
    }

    public class PMTESTSProcedureStep {
        [XmlElement]
        public long PrimaryId { get; set; }
        [XmlElement]
        public long PSTPrimaryId { get; set; }
        [XmlElement]
        public short Order { get; set; }
        [XmlElement]
        public short Code { get; set; }
        [XmlElement(IsNullable = true)]
        public string Title { get; set; }
        [XmlElement(IsNullable = true)]
        public string ProcedureStepTypeDescription { get; set; }
        [XmlElement(IsNullable = true)]
        public string Description { get; set; }
    }

    /// <summary>
    /// Types of ProcedureTest available in AE. 
    /// 1:TestType.PM 
    /// 2:TestType.Safety
    /// 3:TestType.Both
    /// </summary>
    public enum TestType {
        PM = 1,
        Safety = 2,
        Both = 3,
    }
    
    #endregion

    #region ProcedureStep

    public class ProcedureStepInfo : SystemCodeInfo {
        [XmlElement(IsNullable = true)]
        public string Title { get; set; }
        [XmlElement(IsNullable = true)]
        public SystemCode Type { get; set; }
    }
    
    #endregion

    #region ProcedureStepType
    
    public class ProcedureStepTypeInfo : SystemCodeInfo {
        [XmlElement(IsNullable = true)]
        public string Notes { get; set; }
    }

    #endregion

    #region MedTester

    public class MedTesterItemInfo : SystemCodeInfo {
        [XmlElement(IsNullable = true)]
        public string Type { get; set; }
    }

    public class MedTesterChecklistItem {
        [XmlElement(IsNullable = true)]
        public MedTesterItemInfo MedTesterItem { get; set; }
        [XmlElement]
        public long PrimaryId { get; set; }
        [XmlElement]
        public short Order { get; set; }
    }
    
    #endregion

    #region Procedure CheckList Task
    
    public class ChecklistTask {
        [XmlElement]
        public long PrimaryId { get; set; }
        [XmlElement]
        public int Code { get; set; }
        [XmlElement(IsNullable = true)]
        public string Title { get; set; }
        [XmlElement(IsNullable = true)]
        public string CheckListType { get; set; }
    }

    public class ChecklistTaskInfo : ChecklistTask {
        [XmlElement(IsNullable = true)]
        public string Archived { get; set; }
        [XmlElement]
        public decimal TimeEstimate { get; set; }        
    }

    public class ChecklistTaskDetails : ChecklistTaskInfo {
        [XmlElement]
        public decimal PMin { get; set; }
        [XmlElement]
        public decimal PMax { get; set; }
        [XmlElement(IsNullable = true)]
        public string Units { get; set; }
        //Author/Date/ISS-Srikanth Nuvvula/3rd May 2013/ISS-2973,3865
        //Ability to skip or mark NA checklist
        [XmlElement]
        public bool AllowNA { get; set; }
    }

    public class ProcedureChecklistTaskInfo : ChecklistTaskInfo {
        [XmlElement]
        public int Order { get; set; }
    }

    public class ProcedureChecklist : ChecklistTaskInfo {
        [XmlElement]
        public int Order { get; set; }
        [XmlElement]
        public Procedure PMTest { get; set; }
    }

    public enum SortChecklistTasksBy {
        Code = 0,
        Status = 1,
        Type = 2,
        Task = 3,
    }

    #endregion
}