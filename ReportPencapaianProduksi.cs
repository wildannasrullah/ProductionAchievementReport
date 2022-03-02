using System;
using System.Data;
using System.Collections.Generic;
using SIMCore.Models;
using SIMCore.Services;
using SIMCore.Controllers;

namespace Expansion
{
    public class ExpansionHook
	{
        private DB db;
		private SQLService sql;
        private MasterReportViewerController controller;
		
		public ExpansionHook(DB db, SQLService sql, MasterReportViewerController controller)
		{
            this.db = db;
            this.sql = sql;
            this.controller = controller;
		}

        public ExpansionResult OnPost(string action, Dictionary<string, object> dictParameter, params object[] objectParameters)
		{
			try
			{
                /*
                    Console.WriteLine("message"); // Print something in SIMCore console
                    db.Dump(dictParameter); // Dump variable to SIMCore console
				    string role = db.GetCurrentRole(controller.HttpContext)); // Get current role
				    string user = db.GetCurrentUser(controller.HttpContext)); // Get current user
                */

			    //Do something here 	
				controller.tblSource = new DataTable();
				
				//controller.tblSource.Columns.Add("docdate", typeof(DateTime));
				controller.tblSource.Columns.Add("jodocno", typeof(string));
				controller.tblSource.Columns.Add("jrdno", typeof(string));
				controller.tblSource.Columns.Add("mudno", typeof(string));
				controller.tblSource.Columns.Add("operator", typeof(string));
				controller.tblSource.Columns.Add("asstoperator1", typeof(string));
				controller.tblSource.Columns.Add("asstoperator2", typeof(string));
				controller.tblSource.Columns.Add("asstoperator3", typeof(string));
				controller.tblSource.Columns.Add("asstoperator4", typeof(string));
				controller.tblSource.Columns.Add("asstoperator5", typeof(string));
				controller.tblSource.Columns.Add("asstoperator6", typeof(string));
				controller.tblSource.Columns.Add("zsupervisor", typeof(string));
				controller.tblSource.Columns.Add("waktujrd", typeof(string));
				controller.tblSource.Columns.Add("waktumud", typeof(string));
				controller.tblSource.Columns.Add("waktudt", typeof(string));
				controller.tblSource.Columns.Add("nethour", typeof(string));
				controller.tblSource.Columns.Add("machine", typeof(string));
				controller.tblSource.Columns.Add("qty", typeof(string));
				controller.tblSource.Columns.Add("capacityperhour", typeof(string));
				controller.tblSource.Columns.Add("persen", typeof(string));
				
			/*	
				string query = @"SELECT jrh.DocDate, jrh.Docno
					FROM jobresulth AS jrh
					WHERE jrh.Status<>'DELETED' " + dictParameter["additionalCondition"].ToString() + @"
					ORDER BY jrh.DocDate ASC";
			*/
				string query = @"select a.docdate, a.jodocno,a.docno jrdno,c.docno mudno
									   ,a.zoperator as operator,a.zasstoperator as asstoperator1
									   ,a.zasstoperator2 as asstoperator2
									   ,a.zasstoperator3 as asstoperator3
									   ,a.zasstoperator4 as asstoperator4
									   ,a.zasstoperator5 as asstoperator5
									   ,a.zasstoperator6 as asstoperator6
									   ,a.zsupervisor
									   ,date_format(concat(a.docdate, ' ', a.doctime),'%Y-%m-%d %H:%i:%s') waktujrd
									   ,date_format(concat(c.docdate, ' ', c.doctime),'%Y-%m-%d %H:%i:%s') waktumud
									   ,ifnull(d.waktudt,0)waktudt
									   ,(TIME_TO_SEC(TIMEDIFF(concat(a.docdate, ' ', a.doctime),concat(c.docdate, ' ', c.doctime)))/3600)-ifnull(d.waktudt,0) nethour
									   ,a.machine,b.qty
									   ,ifnull(e.capacityperhour,0)capacityperhour
									   ,ifnull(sum(b.qty)
									   /((TIME_TO_SEC(TIMEDIFF(concat(a.docdate, ' ', a.doctime),concat(c.docdate, ' ', c.doctime)))/3600)-ifnull(d.waktudt,0))
									   /ifnull(e.capacityperhour,0),0)*100 persen
								from jobresulth a
								join jobresultd b on a.docno=b.docno
								left join materialusageh c on a.zmudocno=c.docno
								left join (select a.docno,sum(duration) waktudt
									  from jobresulth a
									  join jobresultdt b on a.docno=b.docno
									  where reason in ('mtc_int','mtc_eks','prf_eks',
													   'prf_int','prf_tin','dwn_tgu',
													   'isth','off_btu','off_job',
													   'off_ntk','off_utl')
											and status<>'DELETED'
									  group by docno) d on a.docno=d.docno
								left join mastermachine e on a.machine=e.code
								where a.status<>'DELETED' " + dictParameter["additionalCondition"].ToString() + @"
								group by a.docno
								order by a.jodocno,a.docno,c.docno";
				
				
				DataTable tblHasil = sql.Select(query);
				
				// Fill tblSource
				DataRow newRow = null, lastRow = null;
				DateTime lastDocDate = DateTime.Parse("1/1/1900");
				string lastMachine = "", lastPrintCode = "";
				
				foreach(DataRow row in tblHasil.Rows)
				{
					//if (lastDocDate.Date != ((DateTime)row["DocDate"]).Date)
					//{
						newRow = controller.tblSource.NewRow();
						//newRow["docdate"] 		= row["docdate"];
						newRow["jodocno"] 		= row["jodocno"];
						newRow["jrdno"] 		= row["jrdno"];
						newRow["mudno"]   		= row["mudno"];
						newRow["operator"] 		= row["operator"];
						newRow["asstoperator1"] = row["asstoperator1"];
						newRow["asstoperator2"] = row["asstoperator2"];
						newRow["asstoperator3"] = row["asstoperator3"];
						newRow["asstoperator4"] = row["asstoperator4"];
						newRow["asstoperator5"] = row["asstoperator5"];
						newRow["asstoperator6"] = row["asstoperator6"];
						newRow["zsupervisor"] 	= row["zsupervisor"];
						newRow["waktujrd"] 		= row["waktujrd"];
						newRow["waktumud"] 		= row["waktumud"];
						newRow["waktudt"] 		= row["waktudt"];
						newRow["nethour"] 		= row["nethour"];
						newRow["machine"] 		= row["machine"];
						newRow["qty"] 			= row["qty"];
						newRow["capacityperhour"] = row["capacityperhour"];
						newRow["persen"] 		= row["persen"];
						
						controller.tblSource.Rows.Add(newRow);
						
						if (lastDocDate.Date != ((DateTime)row["docdate"]).Date)
						{
							lastRow = newRow;
						}
					//}
					lastDocDate = ((DateTime)row["docdate"]).Date;
					
				}
				controller.tblSource.AcceptChanges(); 
            } catch(Exception ex)
			{
				return new ExpansionResult(false, ex.Message);
            }
			
			return new ExpansionResult(true, "");
        }
	}
}