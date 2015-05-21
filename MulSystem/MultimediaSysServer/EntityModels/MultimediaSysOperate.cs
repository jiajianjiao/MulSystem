using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultimediaSysServer.EntityModels;
using MultimediaSysServer.EntityClasses;


namespace MultimediaSysServer.EntityModels
{
    public class MultimediaSysOperate
    {
        //
        public List<MaterialManagementDetail> Getjjj_MaterialManagementDetail1(string ThumbnailImgPath)
        {
            var ctx = new MultimediaSYSEntities();
            var jjj_MaterialManagementDetail_EntityModel = (from a in ctx.jjj_MaterialManagementDetail
                                                            where (a.ThumbnailImgPath == ThumbnailImgPath)
                                                            orderby a.UploadTime descending
                                                            select new MaterialManagementDetail { MaterialID = a.MaterialID, FileName = a.FileName, FileSize = a.FileSize, FileType = a.FileType, FilePath = a.FilePath, FileSuffixName = a.FileSuffixName, UploadTime = a.UploadTime, TransformFilePath = a.TransformFilePath, UploadUserId = a.UploadUserId, TransformFileSuffixName = a.TransformFileSuffixName, ThumbnailImgPath = a.ThumbnailImgPath, OriginalFileName = a.OriginalFileName })
                    .Take(1000); //take操作符类似于SQL中的TOP操作符
            List<MaterialManagementDetail> TerInfoEF = jjj_MaterialManagementDetail_EntityModel.ToList();
            return TerInfoEF;
        }
        //
        public int Updatejjj_TerminalSwitchScreenTime1(int TerminalEquipmentID, int IsSend)
        {
            int count = 0;
            using (var ctx = new MultimediaSYSEntities())
            {
                var query = from a in ctx.jjj_TerminalSwitchScreenTime
                            where a.TerminalEquipmentID == TerminalEquipmentID
                            select a;
                foreach (var item in query)
                {
                    item.UpdateTime = DateTime.Now;
                    item.IsSend = IsSend;
                }
                count = ctx.SaveChanges();
            }
            return count;
        }
        public List<TerminalSwitchScreenTime_EntityModel> GetTerminalSwitchScreenTime3(bool IsEnable, int IsSend, int TerminalStatus, int TerminalEquipmentID)
        {
            var ctx = new MultimediaSYSEntities();
            var terminalSwitchScreenTime_EntityModel = (from a in ctx.jjj_TerminalSwitchScreenTime
                                                        join b in ctx.jjj_TerminalEquipmentList on a.TerminalEquipmentID equals b.TerminalEquipmentID
                                                        where (a.IsEnable == IsEnable) && (b.TerminalStatus == TerminalStatus) && (a.IsSend == IsSend) && (a.TerminalEquipmentID == TerminalEquipmentID)
                                                        orderby a.TerminalSwitchScreenTimeID descending
                                                        select new TerminalSwitchScreenTime_EntityModel { TerminalSwitchScreenTimeID = a.TerminalSwitchScreenTimeID, TerminalEquipmentID = a.TerminalEquipmentID, SessionId = b.SessionId, TurnOnTime = a.TurnOnTime, TurnOffTime = a.TurnOffTime, IsMonday = a.IsMonday, IsTuesday = a.IsTuesday, IsWednesday = a.IsWednesday, IsThursday = a.IsThursday, IsFriday = a.IsFriday, IsSaturday = a.IsSaturday, IsSunday = a.IsSunday, IsEnable = a.IsEnable, UpdateTime = a.UpdateTime, IsSend = IsSend })
                    .Take(1000); //take操作符类似于SQL中的TOP操作符
            List<TerminalSwitchScreenTime_EntityModel> TerInfoEF = terminalSwitchScreenTime_EntityModel.ToList();
            return TerInfoEF;
        }
        public List<TerminalSwitchScreenTime_EntityModel> GetTerminalSwitchScreenTime1(bool IsEnable, int IsSend, int TerminalStatus)
        {
            var ctx = new MultimediaSYSEntities();
            var terminalSwitchScreenTime_EntityModel = (from a in ctx.jjj_TerminalSwitchScreenTime
                                                        join b in ctx.jjj_TerminalEquipmentList on a.TerminalEquipmentID equals b.TerminalEquipmentID
                                                        where (a.IsEnable == IsEnable) && (b.TerminalStatus == TerminalStatus) && (a.IsSend == IsSend)
                                                        orderby a.TerminalSwitchScreenTimeID descending
                                                        select new TerminalSwitchScreenTime_EntityModel { TerminalSwitchScreenTimeID = a.TerminalSwitchScreenTimeID, TerminalEquipmentID = a.TerminalEquipmentID, SessionId = b.SessionId, TurnOnTime = a.TurnOnTime, TurnOffTime = a.TurnOffTime, IsMonday = a.IsMonday, IsTuesday = a.IsTuesday, IsWednesday = a.IsWednesday, IsThursday = a.IsThursday, IsFriday = a.IsFriday, IsSaturday = a.IsSaturday, IsSunday = a.IsSunday, IsEnable = a.IsEnable, UpdateTime = a.UpdateTime, IsSend = IsSend })
                    .Take(1000); //take操作符类似于SQL中的TOP操作符
            List<TerminalSwitchScreenTime_EntityModel> TerInfoEF = terminalSwitchScreenTime_EntityModel.ToList();
            return TerInfoEF;
        }
        public List<IGrouping<int?, TerminalSwitchScreenTime_EntityModel>> GetTerminalSwitchScreenTime2(bool IsEnable, int IsSend, int TerminalStatus)
        {
            var ctx = new MultimediaSYSEntities();
            var terminalSwitchScreenTime_EntityModel = (from a in ctx.jjj_TerminalSwitchScreenTime
                                                        join b in ctx.jjj_TerminalEquipmentList on a.TerminalEquipmentID equals b.TerminalEquipmentID
                                                        where (a.IsEnable == IsEnable) && (b.TerminalStatus == TerminalStatus) && (a.IsSend == IsSend)
                                                        orderby a.TerminalSwitchScreenTimeID descending
                                                        select new TerminalSwitchScreenTime_EntityModel { TerminalSwitchScreenTimeID = a.TerminalSwitchScreenTimeID, TerminalEquipmentID = a.TerminalEquipmentID, SessionId = b.SessionId, TurnOnTime = a.TurnOnTime, TurnOffTime = a.TurnOffTime, IsMonday = a.IsMonday, IsTuesday = a.IsTuesday, IsWednesday = a.IsWednesday, IsThursday = a.IsThursday, IsFriday = a.IsFriday, IsSaturday = a.IsSaturday, IsSunday = a.IsSunday, IsEnable = a.IsEnable, UpdateTime = a.UpdateTime, IsSend = IsSend })
                    .Take(1000); //take操作符类似于SQL中的TOP操作符
            var query = from a in terminalSwitchScreenTime_EntityModel group a by a.TerminalEquipmentID into g select g;
            return query.ToList();
        }
        //
        public int AddUser_DrinkCostSerialList(string SerialNumber, string IcCardNo, string CostTime, string DeviceCode, string CostFee, string RunningWaterLitre, string CurrentUserBalance)
        {
            int count = 0;
            using (var ctx = new MultimediaSYSEntities())
            {
                var DrinkCostSeria_tmp = new jjj_user_DrinkCostSerialList { SerialNumber = SerialNumber, IcCardNo = IcCardNo, CostTime = Convert.ToDateTime(CostTime), DeviceCode = DeviceCode, CostFee = Convert.ToDecimal(CostFee), RunningWaterLitre = Convert.ToDecimal(RunningWaterLitre), CurrentUserBalance = Convert.ToDecimal(CurrentUserBalance) };
                ctx.jjj_user_DrinkCostSerialList.Add(DrinkCostSeria_tmp);
                count = ctx.SaveChanges();
            }
            return count;
        }
        //

        /// <summary>
        /// 添加一条用户基础信息
        /// </summary>
        /// <param name="usernameTmp"></param>
        /// <param name="passwordTmp"></param>
        /// <param name="phoneTmp"></param>
        /// <param name="isEnableTmp"></param>
        public int AddUserInfo1(string usernameTmp, string passwordTmp, string phoneTmp, int isEnableTmp)
        {
            int count = 0;
            using (var ctx = new MultimediaSYSEntities())
            {
                var userinfo_tmp = new jjj_users { username = usernameTmp, password = passwordTmp, phone = phoneTmp, isEnable = isEnableTmp };
                ctx.jjj_users.Add(userinfo_tmp);
                count = ctx.SaveChanges();
            }
            return count;
        }

        public int DeleteUserInfo1(int uidTmp)
        {
            int count = 0;
            using (var ctx = new MultimediaSYSEntities())
            {
                var query = from a in ctx.jjj_users
                            where a.uid == uidTmp
                            select a;
                foreach (var item in query)
                {
                    ctx.jjj_users.Remove(item);
                }
                count = ctx.SaveChanges();
            }
            return count;
        }

        public int UpdateUserPassword1(int uidTmp, string passwordTmp)
        {
            int count = 0;
            using (var ctx = new MultimediaSYSEntities())
            {
                var query = from a in ctx.jjj_users
                            where a.uid == uidTmp
                            select a;
                foreach (var item in query)
                {
                    item.password = passwordTmp;
                }
                count = ctx.SaveChanges();
            }
            return count;
        }

        /// <summary>
        /// 查询一组用户信息集
        /// </summary>
        /// <param name="usernameTmp"></param>
        /// <returns></returns>
        public List<UserInfo_EntityModel> GetUserInfo1(string usernameTmp)
        {
            var ctx = new MultimediaSYSEntities();
            var userInfo = (from a in ctx.jjj_users
                            join b in ctx.jjj_usergroups on a.groupid equals b.groupid
                            where a.username == usernameTmp
                            orderby a.uid descending
                            select new UserInfo_EntityModel { uid = a.uid, groupid = b.groupid, password = a.password, username = a.username, grouptitle = b.grouptitle, actualname = a.actualname, phone = a.phone, HengYuCode = a.HengYuCode, isEnable = a.isEnable })
                             .Take(1000); //take操作符类似于SQL中的TOP操作符
            List<UserInfo_EntityModel> userInfoEF = userInfo.ToList(); //userInfo2.ToList().FirstOrDefault()写法为返回第一个实体对象
            return userInfoEF;
        }

        public List<TerminalEquipmentInfo_EntityModel> GetTerminalEquipmentInfo1(string deviceCodeTmp)
        {
            var ctx = new MultimediaSYSEntities();
            var terInfo = (from a in ctx.jjj_TerminalEquipmentList
                           join b in ctx.jjj_TerminalEquipmentCategory on a.TerminalEquipmentCategoryId equals b.TerminalEquipmentCategoryId
                           where a.DeviceCode == deviceCodeTmp
                           orderby a.Layer descending
                           select new TerminalEquipmentInfo_EntityModel { TerminalEquipmentID = a.TerminalEquipmentID, TerminalEquipmentCategoryId = b.TerminalEquipmentCategoryId, CurrentUserId = a.CurrentUserId, TerminalStatus = a.TerminalStatus, IsRegister = a.IsRegister, Layer = a.Layer, AdvanceDownNum = a.AdvanceDownNum, SessionId = a.SessionId, TerminalVersion = a.TerminalVersion, TerminalIP = a.TerminalIP, TerminalPlatform = a.TerminalPlatform, DeviceCode = a.DeviceCode, TerminalName = a.TerminalName, MainServerIP = a.MainServerIP, MainServerPort = a.MainServerPort, TerminalEquipmentCategoryName = b.TerminalEquipmentCategoryName })
                             .Take(1000); //take操作符类似于SQL中的TOP操作符
            List<TerminalEquipmentInfo_EntityModel> terInfoEF = terInfo.ToList(); //userInfo2.ToList().FirstOrDefault()写法为返回第一个实体对象
            return terInfoEF;
        }
        public List<TerminalEquipmentInfo_EntityModel> GetTerminalEquipmentInfo2(int TerminalEquipmentID)
        {
            var ctx = new MultimediaSYSEntities();
            var terInfo = (from a in ctx.jjj_TerminalEquipmentList
                           join b in ctx.jjj_TerminalEquipmentCategory on a.TerminalEquipmentCategoryId equals b.TerminalEquipmentCategoryId
                           where a.TerminalEquipmentID == TerminalEquipmentID
                           orderby a.Layer descending
                           select new TerminalEquipmentInfo_EntityModel { TerminalEquipmentID = a.TerminalEquipmentID, TerminalEquipmentCategoryId = b.TerminalEquipmentCategoryId, CurrentUserId = a.CurrentUserId, TerminalStatus = a.TerminalStatus, IsRegister = a.IsRegister, Layer = a.Layer, AdvanceDownNum = a.AdvanceDownNum, SessionId = a.SessionId, TerminalVersion = a.TerminalVersion, TerminalIP = a.TerminalIP, TerminalPlatform = a.TerminalPlatform, DeviceCode = a.DeviceCode, TerminalName = a.TerminalName, MainServerIP = a.MainServerIP, MainServerPort = a.MainServerPort, TerminalEquipmentCategoryName = b.TerminalEquipmentCategoryName })
                             .Take(1000); //take操作符类似于SQL中的TOP操作符
            List<TerminalEquipmentInfo_EntityModel> terInfoEF = terInfo.ToList(); //userInfo2.ToList().FirstOrDefault()写法为返回第一个实体对象
            return terInfoEF;
        }
        public List<TerminalEquipmentInfo_EntityModel> GetTerminalEquipmentInfo3(int IsSend, int TerminalStatus)
        {
            var ctx = new MultimediaSYSEntities();
            var terInfo = (from a in ctx.jjj_TerminalEquipmentList
                           join b in ctx.jjj_TerminalEquipmentCategory on a.TerminalEquipmentCategoryId equals b.TerminalEquipmentCategoryId
                           where a.IsSend == IsSend
                           orderby a.Layer descending
                           select new TerminalEquipmentInfo_EntityModel { TerminalEquipmentID = a.TerminalEquipmentID, TerminalEquipmentCategoryId = b.TerminalEquipmentCategoryId, CurrentUserId = a.CurrentUserId, TerminalStatus = a.TerminalStatus, IsRegister = a.IsRegister, Layer = a.Layer, AdvanceDownNum = a.AdvanceDownNum, SessionId = a.SessionId, TerminalVersion = a.TerminalVersion, TerminalIP = a.TerminalIP, TerminalPlatform = a.TerminalPlatform, DeviceCode = a.DeviceCode, TerminalName = a.TerminalName, MainServerIP = a.MainServerIP, MainServerPort = a.MainServerPort, TerminalEquipmentCategoryName = b.TerminalEquipmentCategoryName, DownloadStartTime = a.DownloadStartTime, DownloadEndTime = a.DownloadEndTime })
                             .Take(1000); //take操作符类似于SQL中的TOP操作符
            List<TerminalEquipmentInfo_EntityModel> terInfoEF = terInfo.ToList(); //userInfo2.ToList().FirstOrDefault()写法为返回第一个实体对象
            return terInfoEF;
        }
        public int UpdateTerminalEquipmentInfo3(int TerminalEquipmentIDTmp, int IsSendTmp)
        {
            int count = 0;
            using (var ctx = new MultimediaSYSEntities())
            {
                var query = from a in ctx.jjj_TerminalEquipmentList
                            where a.TerminalEquipmentID == TerminalEquipmentIDTmp
                            select a;
                foreach (var item in query)
                {
                    item.IsSend = IsSendTmp;
                }
                count = ctx.SaveChanges();
            }
            return count;
        }
        public int UpdateTerminalEquipmentInfo1(string deviceCodeTmp, string sessionIDTmp)
        {
            int count = 0;
            using (var ctx = new MultimediaSYSEntities())
            {
                var query = from a in ctx.jjj_TerminalEquipmentList
                            where a.DeviceCode == deviceCodeTmp
                            select a;
                foreach (var item in query)
                {
                    item.SessionId = sessionIDTmp;
                    item.TerminalStatus = 1;
                }
                count = ctx.SaveChanges();
            }
            return count;
        }

        public int UpdateTerminalEquipmentInfo2(string sessionIDTmp)
        {
            int count = 0;
            using (var ctx = new MultimediaSYSEntities())
            {
                var query = from a in ctx.jjj_TerminalEquipmentList
                            where a.SessionId == sessionIDTmp
                            select a;
                foreach (var item in query)
                {
                    item.TerminalStatus = 0;
                }
                count = ctx.SaveChanges();
            }
            return count;
        }
        //
        public List<TerminalEquipmentSerialCommand_EntityModel> GetTerminalEquipmentSerialCommand1(int TerminalStatus, int SendStatus)
        {
            var ctx = new MultimediaSYSEntities();
            var TerminalEquipmentSerialCommand = (from a in ctx.jjj_TerminalEquipmentSerialCommand
                                                  join b in ctx.jjj_TerminalEquipmentList on a.TerminalEquipmentID equals b.TerminalEquipmentID
                                                  where (a.SendStatus == SendStatus) && (b.TerminalStatus == TerminalStatus)
                                                  orderby a.SendTime descending
                                                  select new TerminalEquipmentSerialCommand_EntityModel { TerSerialCommandID = a.TerSerialCommandID, SendAdminName = a.SendAdminName, SendTime = a.SendTime, TerminalEquipmentID = a.TerminalEquipmentID, TerminalStatus = b.TerminalStatus, SessionId = b.SessionId, DeviceCode = b.DeviceCode, TerminalName = b.TerminalName, CommandTypeId = a.CommandTypeId, SendContent = a.SendContent, SendStatus = a.SendStatus, ReceiveBackTime = a.ReceiveBackTime })
                    .Take(1000); //take操作符类似于SQL中的TOP操作符
            List<TerminalEquipmentSerialCommand_EntityModel> TerInfoEF = TerminalEquipmentSerialCommand.ToList(); //userInfo2.ToList().FirstOrDefault()写法为返回第一个实体对象
            return TerInfoEF;
        }
        public int UpdateTerminalEquipmentSerialCommand2(int TerSerialCommandID, int SendStatus)
        {
            int count = 0;
            using (var ctx = new MultimediaSYSEntities())
            {
                var query = from a in ctx.jjj_TerminalEquipmentSerialCommand
                            where a.TerSerialCommandID == TerSerialCommandID
                            select a;
                foreach (var item in query)
                {
                    item.SendStatus = SendStatus;
                }
                count = ctx.SaveChanges();
            }
            return count;
        }
        public int UpdateTerminalEquipmentSerialCommand1(int TerSerialCommandID, int SendStatus, Nullable<DateTime> ReceiveBackTime)
        {
            int count = 0;
            using (var ctx = new MultimediaSYSEntities())
            {
                var query = from a in ctx.jjj_TerminalEquipmentSerialCommand
                            where a.TerSerialCommandID == TerSerialCommandID
                            select a;
                foreach (var item in query)
                {
                    item.SendStatus = SendStatus;
                    item.ReceiveBackTime = ReceiveBackTime;
                }
                count = ctx.SaveChanges();
            }
            return count;
        }
        //开始进行节目制作发送的相关方法
        public List<ProgramSendSerialList_EntityModel> GetProgramSendSerialList1(int TerminalStatus, int SendStatus)
        {
            var ctx = new MultimediaSYSEntities();
            var programSendSerialList_EntityModel = (from a in ctx.jjj_ProgramSendSerialList
                                                     join b in ctx.jjj_TerminalEquipmentList on a.TerminalEquipmentID equals b.TerminalEquipmentID
                                                     where (a.SendStatus == SendStatus) && (b.TerminalStatus == TerminalStatus)
                                                     orderby a.CreateTime descending
                                                     select new ProgramSendSerialList_EntityModel { ProgramSendSerialID = a.ProgramSendSerialID, TerminalEquipmentID = a.TerminalEquipmentID, ProgramInfoID = a.ProgramInfoID, SendStatus = a.SendStatus, EfficaciousDateStart = a.EfficaciousDateStart, SessionId = b.SessionId, DeviceCode = b.DeviceCode, TerminalName = b.TerminalName, EfficaciousDateEnd = a.EfficaciousDateEnd, EfficaciousTimeStart = a.EfficaciousTimeStart, EfficaciousTimeEnd = a.EfficaciousTimeEnd, ReceiveBackTime = a.ReceiveBackTime })
                    .Take(1000); //take操作符类似于SQL中的TOP操作符
            List<ProgramSendSerialList_EntityModel> TerInfoEF = programSendSerialList_EntityModel.ToList();
            return TerInfoEF;
        }

        public List<ProgramInfoList_EntityModel> GetProgramInfoList1(int ProgramInfoID)
        {
            var ctx = new MultimediaSYSEntities();
            var programInfoList_EntityModel = (from a in ctx.jjj_ProgramInfoList
                                               where (a.ProgramInfoID == ProgramInfoID) && (a.ProgramStatus == 1)
                                               orderby a.CreateTime descending
                                               select new ProgramInfoList_EntityModel { ProgramInfoID = a.ProgramInfoID, ProgramWidth = a.ProgramWidth, ProgramHeight = a.ProgramHeight, ProgramStatus = a.ProgramStatus, CreateUserId = a.CreateUserId, ProgramTitle = a.ProgramTitle, CreateTime = a.CreateTime, UpdateTime = a.UpdateTime })
                    .Take(1000); //take操作符类似于SQL中的TOP操作符
            List<ProgramInfoList_EntityModel> TerInfoEF = programInfoList_EntityModel.ToList();
            return TerInfoEF;
        }
        //
        public List<IGrouping<int?, ProgramInfoListItem_EntityModel>> GetProgramInfoListItem3(int ProgramInfoID)
        {
            var ctx = new MultimediaSYSEntities();
            var programInfoListItem_EntityModel = (from a in ctx.jjj_ProgramInfoListItem
                                                   join b in ctx.jjj_ProgramInfoList on a.ProgramInfoID equals b.ProgramInfoID
                                                   where (a.ProgramInfoID == ProgramInfoID)
                                                   orderby a.ProgramInfoItemLayer descending
                                                   select new ProgramInfoListItem_EntityModel { ProgramInfoItemLayer = a.ProgramInfoItemLayer })
                    .Take(1000);
            var query = from a in programInfoListItem_EntityModel group a by a.ProgramInfoItemLayer into g select g;
            return query.ToList();
        }
        public List<ProgramInfoListItem_EntityModel> GetProgramInfoListItem2(int ProgramInfoID, int ProgramInfoItemLayer)
        {
            var ctx = new MultimediaSYSEntities();
            var programInfoListItem_EntityModel = (from a in ctx.jjj_ProgramInfoListItem
                                                   join b in ctx.jjj_ProgramInfoList on a.ProgramInfoID equals b.ProgramInfoID
                                                   where (a.ProgramInfoID == ProgramInfoID) && (a.ProgramInfoItemLayer == ProgramInfoItemLayer)
                                                   orderby a.ProgramInfoItemLayer descending
                                                   select new ProgramInfoListItem_EntityModel { ProgramInfoItemID = a.ProgramInfoItemID, ProgramInfoID = a.ProgramInfoID, ProgramInfoItemType = a.ProgramInfoItemType, ProgramInfoItemWidth = a.ProgramInfoItemWidth, ProgramInfoItemHeight = a.ProgramInfoItemHeight, ProgramInfoItemLayer = a.ProgramInfoItemLayer, ProgramInfoItemTitle = a.ProgramInfoItemTitle, ProgramInfoItemLeft = a.ProgramInfoItemLeft, ProgramInfoItemTop = a.ProgramInfoItemTop, IsShowWater = a.IsShowWater })
                    .Take(1000); //take操作符类似于SQL中的TOP操作符
            List<ProgramInfoListItem_EntityModel> TerInfoEF = programInfoListItem_EntityModel.ToList();
            return TerInfoEF;
        }

        public List<ProgramInfoListItem_EntityModel> GetProgramInfoListItem1(int ProgramInfoID)
        {
            var ctx = new MultimediaSYSEntities();
            var programInfoListItem_EntityModel = (from a in ctx.jjj_ProgramInfoListItem
                                                   join b in ctx.jjj_ProgramInfoList on a.ProgramInfoID equals b.ProgramInfoID
                                                   where (a.ProgramInfoID == ProgramInfoID)
                                                   orderby a.ProgramInfoItemLayer descending
                                                   select new ProgramInfoListItem_EntityModel { ProgramInfoItemID = a.ProgramInfoItemID, ProgramInfoID = a.ProgramInfoID, ProgramInfoItemType = a.ProgramInfoItemType, ProgramInfoItemWidth = a.ProgramInfoItemWidth, ProgramInfoItemHeight = a.ProgramInfoItemHeight, ProgramInfoItemLayer = a.ProgramInfoItemLayer, ProgramInfoItemTitle = a.ProgramInfoItemTitle, ProgramInfoItemLeft = a.ProgramInfoItemLeft, ProgramInfoItemTop = a.ProgramInfoItemTop })
                    .Take(1000); //take操作符类似于SQL中的TOP操作符
            List<ProgramInfoListItem_EntityModel> TerInfoEF = programInfoListItem_EntityModel.ToList();
            return TerInfoEF;
        }
        public List<ProgramInfoItem_DocumentDetail_EntityModel> GetProgramInfoItem_DocumentDetail1(int ProgramInfoItemID)
        {
            var ctx = new MultimediaSYSEntities();
            var programInfoItem_DocumentDetail_EntityModel = (from a in ctx.jjj_ProgramInfoItem_DocumentDetail
                                                              join b in ctx.jjj_ProgramInfoListItem on a.ProgramInfoItemID equals b.ProgramInfoItemID
                                                              where (a.ProgramInfoItemID == ProgramInfoItemID)
                                                              orderby a.ProgramInfoItemID descending
                                                              select new ProgramInfoItem_DocumentDetail_EntityModel { ID = a.ID, ProgramInfoItemID = a.ProgramInfoItemID, DocumentLayer = a.DocumentLayer, TimeLength = a.TimeLength, CartoonType = a.CartoonType, DocumentUrl = a.DocumentUrl })
                    .Take(1000); //take操作符类似于SQL中的TOP操作符
            List<ProgramInfoItem_DocumentDetail_EntityModel> TerInfoEF = programInfoItem_DocumentDetail_EntityModel.ToList();
            return TerInfoEF;
        }
        public List<ProgramInfoItem_ImageDetail_EntityModel> GetProgramInfoItem_ImageDetail1(int ProgramInfoItemID)
        {
            var ctx = new MultimediaSYSEntities();
            var programInfoItem_ImageDetail_EntityModel = (from a in ctx.jjj_ProgramInfoItem_ImageDetail
                                                           join b in ctx.jjj_ProgramInfoListItem on a.ProgramInfoItemID equals b.ProgramInfoItemID
                                                           where (a.ProgramInfoItemID == ProgramInfoItemID)
                                                           orderby a.ProgramInfoItemID descending
                                                           select new ProgramInfoItem_ImageDetail_EntityModel { ID = a.ID, ProgramInfoItemID = a.ProgramInfoItemID, ImageLayer = a.ImageLayer, TimeLength = a.TimeLength, CartoonType = a.CartoonType, ImageUrl = a.ImageUrl })
                    .Take(1000); //take操作符类似于SQL中的TOP操作符
            List<ProgramInfoItem_ImageDetail_EntityModel> TerInfoEF = programInfoItem_ImageDetail_EntityModel.ToList();
            return TerInfoEF;
        }

        public List<ProgramInfoItem_TextDetail_EntityModel> GetProgramInfoItem_TextDetail1(int ProgramInfoItemID)
        {
            var ctx = new MultimediaSYSEntities();
            var programInfoItem_TextDetail_EntityModel = (from a in ctx.jjj_ProgramInfoItem_TextDetail
                                                          join b in ctx.jjj_ProgramInfoListItem on a.ProgramInfoItemID equals b.ProgramInfoItemID
                                                          where (a.ProgramInfoItemID == ProgramInfoItemID)
                                                          orderby a.ProgramInfoItemID descending
                                                          select new ProgramInfoItem_TextDetail_EntityModel { ID = a.ID, ProgramInfoItemID = a.ProgramInfoItemID, TextLayer = a.TextLayer, TextDirectionType = a.TextDirectionType, IsBackgroundTransparent = a.IsBackgroundTransparent, IsBold = a.IsBold, FontSize = a.FontSize, PlayCount = a.PlayCount, PlaySpeed = a.PlaySpeed, TextContent = a.TextContent, BackgroundColor = a.BackgroundColor, foreColor = a.foreColor, FontFamily = a.FontFamily })
                    .Take(1000); //take操作符类似于SQL中的TOP操作符
            List<ProgramInfoItem_TextDetail_EntityModel> TerInfoEF = programInfoItem_TextDetail_EntityModel.ToList();
            return TerInfoEF;
        }

        public List<ProgramInfoItem_TimeDetail_EntityModel> GetProgramInfoItem_TimeDetail1(int ProgramInfoItemID)
        {
            var ctx = new MultimediaSYSEntities();
            var programInfoItem_TimeDetail_EntityModel = (from a in ctx.jjj_ProgramInfoItem_TimeDetail
                                                          join b in ctx.jjj_ProgramInfoListItem on a.ProgramInfoItemID equals b.ProgramInfoItemID
                                                          where (a.ProgramInfoItemID == ProgramInfoItemID)
                                                          orderby a.ProgramInfoItemID descending
                                                          select new ProgramInfoItem_TimeDetail_EntityModel { ID = a.ID, ProgramInfoItemID = a.ProgramInfoItemID, FontSize = a.FontSize, IsBackgroundTransparent = a.IsBackgroundTransparent, IsBold = a.IsBold, FontFamily = a.FontFamily, FontFormat = a.FontFormat, BackgroundColor = a.BackgroundColor, foreColor = a.foreColor })
                    .Take(1000); //take操作符类似于SQL中的TOP操作符
            List<ProgramInfoItem_TimeDetail_EntityModel> TerInfoEF = programInfoItem_TimeDetail_EntityModel.ToList();
            return TerInfoEF;
        }

        public List<ProgramInfoItem_VideoDetail_EntityModel> GetProgramInfoItem_VideoDetail1(int ProgramInfoItemID)
        {
            var ctx = new MultimediaSYSEntities();
            var programInfoItem_VideoDetail_EntityModel = (from a in ctx.jjj_ProgramInfoItem_VideoDetail
                                                           join b in ctx.jjj_ProgramInfoListItem on a.ProgramInfoItemID equals b.ProgramInfoItemID
                                                           where (a.ProgramInfoItemID == ProgramInfoItemID)
                                                           orderby a.ProgramInfoItemID descending
                                                           select new ProgramInfoItem_VideoDetail_EntityModel { ID = a.ID, ProgramInfoItemID = a.ProgramInfoItemID, VideoLayer = a.VideoLayer, VideoVoice = a.VideoVoice, VideoUrl = a.VideoUrl })
                    .Take(1000); //take操作符类似于SQL中的TOP操作符
            List<ProgramInfoItem_VideoDetail_EntityModel> TerInfoEF = programInfoItem_VideoDetail_EntityModel.ToList();
            return TerInfoEF;
        }

        public int Updatejjj_ProgramSendSerialList1(int ProgramSendSerialID, int SendStatus)
        {
            int count = 0;
            using (var ctx = new MultimediaSYSEntities())
            {
                var query = from a in ctx.jjj_ProgramSendSerialList
                            where a.ProgramSendSerialID == ProgramSendSerialID
                            select a;
                foreach (var item in query)
                {
                    item.SendStatus = SendStatus;
                }
                count = ctx.SaveChanges();
            }
            return count;
        }
        public int Updatejjj_ProgramSendSerialList2(int ProgramSendSerialID, int SendStatus, Nullable<DateTime> ReceiveBackTime)
        {
            int count = 0;
            using (var ctx = new MultimediaSYSEntities())
            {
                var query = from a in ctx.jjj_ProgramSendSerialList
                            where a.ProgramSendSerialID == ProgramSendSerialID
                            select a;
                foreach (var item in query)
                {
                    item.SendStatus = SendStatus;
                    item.ReceiveBackTime = ReceiveBackTime;
                }
                count = ctx.SaveChanges();
            }
            return count;
        }
        public int Updatejjj_ProgramSendSerialList3(int ProgramSendSerialID, int SendStatus, Nullable<DateTime> DownloadOverTime)
        {
            int count = 0;
            using (var ctx = new MultimediaSYSEntities())
            {
                var query = from a in ctx.jjj_ProgramSendSerialList
                            where a.ProgramSendSerialID == ProgramSendSerialID
                            select a;
                foreach (var item in query)
                {
                    item.SendStatus = SendStatus;
                    item.DownloadOverTime = DownloadOverTime;
                }
                count = ctx.SaveChanges();
            }
            return count;
        }
    }
}
