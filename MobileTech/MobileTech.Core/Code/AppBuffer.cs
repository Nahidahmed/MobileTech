using System; using System.IO; using System.Data; using System.Collections; using System.Linq; using Mono.Data.Sqlite; using System.Reflection; using System.Net; using System.Collections.Generic; using MonoTouch.Foundation; using MobileTech.Models.Entity;

namespace MobileTech.Core
{
	public class AppBuffer
	{ 		private static Stack<WorkOrderDetails> _workOrder = new Stack<WorkOrderDetails>(); 		public enum enumNavPage {None =0, TimeEntry=1}; 		private static Stack<enumNavPage> _navPage = new Stack<enumNavPage>();
		public AppBuffer (){
		}  		public static WorkOrderDetails workOder 		{ 			get{ 				//_workOrder = _workOrder ?? new Stack<WorkOrder> (); 				if (_workOrder.Count > 0) 					return _workOrder.Pop (); 				else 					return null; 			} 			set{ 				if (_workOrder.Count > 0) 					_workOrder.Clear (); 				_workOrder.Push (value); 			} 		}  		public static enumNavPage navPage 		{ 			get{  				if (_navPage.Count > 0) 					return _navPage.Pop (); 				else 					return enumNavPage.None; 			} 			set{ _navPage.Push (value); } 		}
	}
}

