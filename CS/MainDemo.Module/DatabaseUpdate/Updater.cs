using System;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using MainDemo.Module.BusinessObjects;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;

namespace MainDemo.Module.DatabaseUpdate {
    public class Updater : DevExpress.ExpressApp.Updating.ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            //Create departments.
            Department devDepartment = ObjectSpace.FindObject<Department>(CriteriaOperator.Parse("Title == 'R&D'"));
            if(devDepartment == null) {
                devDepartment = ObjectSpace.CreateObject<Department>();
                devDepartment.Title = "R&D";
                devDepartment.Office = "1";
                devDepartment.Save();
            }
            Department supDepartment = ObjectSpace.FindObject<Department>(CriteriaOperator.Parse("Title == 'Technical Support'"));
            if(supDepartment == null) {
                supDepartment = ObjectSpace.CreateObject<Department>();
                supDepartment.Title = "Technical Support";
                supDepartment.Office = "2";
                supDepartment.Save();
            }
            Department mngDepartment = ObjectSpace.FindObject<Department>(CriteriaOperator.Parse("Title == 'Management'"));
            if(mngDepartment == null) {
                mngDepartment = ObjectSpace.CreateObject<Department>();
                mngDepartment.Title = "Management";
                mngDepartment.Office = "3";
                mngDepartment.Save();
            }
            //Create employees.
            //Admin is a god that can do everything.
            Employee administrator = ObjectSpace.FindObject<Employee>(CriteriaOperator.Parse("UserName == 'Admin'"));
            if(administrator == null) {
                administrator = ObjectSpace.CreateObject<Employee>();
                administrator.UserName = "Admin";
                administrator.FirstName = "Admin";
                administrator.LastName = "Admin";
                administrator.Department = mngDepartment;
                administrator.IsActive = true;
                administrator.SetPassword("");
                administrator.Roles.Add(GetAdministratorRole());
                administrator.Save();
            }
            //Sam is a manager and he can do everything with his own department
            Employee managerSam = ObjectSpace.FindObject<Employee>(CriteriaOperator.Parse("UserName == 'Sam'"));
            if(managerSam == null) {
                managerSam = ObjectSpace.CreateObject<Employee>();
                managerSam.UserName = "Sam";
                managerSam.FirstName = "Sam";
                managerSam.LastName = "Jackson";
                managerSam.IsActive = true;
                managerSam.SetPassword("");
                managerSam.Department = devDepartment;
                managerSam.Roles.Add(GetManagerRole());
                managerSam.Roles.Add(GetUserRole());
                managerSam.Save();
            }
            //John is an ordinary user within the Sam's department.
            Employee userJohn = ObjectSpace.FindObject<Employee>(CriteriaOperator.Parse("UserName == 'John'"));
            if(userJohn == null) {
                userJohn = ObjectSpace.CreateObject<Employee>();
                userJohn.UserName = "John";
                userJohn.FirstName = "John";
                userJohn.LastName = "Doe";
                userJohn.IsActive = true;
                userJohn.SetPassword("");
                userJohn.Department = devDepartment;
                userJohn.Roles.Add(GetUserRole());
                userJohn.Save();
            }
            //Mary is a manager of another department.  
            Employee managerMary = ObjectSpace.FindObject<Employee>(CriteriaOperator.Parse("UserName == 'Mary'"));
            if(managerMary == null) {
                managerMary = ObjectSpace.CreateObject<Employee>();
                managerMary.UserName = "Mary";
                managerMary.FirstName = "Mary";
                managerMary.LastName = "Tellinson";
                managerMary.IsActive = true;
                managerMary.SetPassword("");
                managerMary.Department = supDepartment;
                managerMary.Roles.Add(GetManagerRole());
                managerMary.Roles.Add(GetUserRole());
                managerMary.Save();
            }
            //Joe is an ordinary user within the Mary's department.
            Employee userJoe = ObjectSpace.FindObject<Employee>(CriteriaOperator.Parse("UserName == 'Joe'"));
            if(userJoe == null) {
                userJoe = ObjectSpace.CreateObject<Employee>();
                userJoe.UserName = "Joe";
                userJoe.FirstName = "Joe";
                userJoe.LastName = "Pitt";
                userJoe.IsActive = true;
                userJoe.SetPassword("");
                userJoe.Department = supDepartment;
                userJoe.Roles.Add(GetUserRole());
                userJoe.Save();
            }
            //Create tasks for employees.
            if(ObjectSpace.FindObject<EmployeeTask>(CriteriaOperator.Parse("Subject == 'Do homework'")) == null) {
                EmployeeTask task = ObjectSpace.CreateObject<EmployeeTask>();
                task.Subject = "Do homework";
                task.AssignedTo = managerSam;
                task.DueDate = DateTime.Now;
                task.Status = TaskStatus.NotStarted;
                task.Description = "This is a task for Sam";
                task.Save();
            }
            if(ObjectSpace.FindObject<EmployeeTask>(CriteriaOperator.Parse("Subject == 'Prepare coffee for everyone'")) == null) {
                EmployeeTask task = ObjectSpace.CreateObject<EmployeeTask>();
                task.Subject = "Prepare coffee for everyone";
                task.AssignedTo = userJohn;
                task.DueDate = DateTime.Now;
                task.Status = TaskStatus.InProgress;
                task.Description = "This is a task for John";
                task.Save();
            }
            if(ObjectSpace.FindObject<EmployeeTask>(CriteriaOperator.Parse("Subject == 'Read latest news'")) == null) {
                EmployeeTask task = ObjectSpace.CreateObject<EmployeeTask>();
                task.Subject = "Read latest news";
                task.AssignedTo = managerMary;
                task.DueDate = DateTime.Now;
                task.Status = TaskStatus.Completed;
                task.Description = "This is a task for Mary";
                task.Save();
            }
            if(ObjectSpace.FindObject<EmployeeTask>(CriteriaOperator.Parse("Subject == 'Book tickets'")) == null) {
                EmployeeTask task = ObjectSpace.CreateObject<EmployeeTask>();
                task.Subject = "Book tickets";
                task.AssignedTo = userJoe;
                task.DueDate = DateTime.Now;
                task.Status = TaskStatus.Deferred;
                task.Description = "This is a task for Joe";
                task.Save();
            }
            ObjectSpace.CommitChanges();
        }

        //Administrators can do everything within the application.
        private PermissionPolicyRole GetAdministratorRole() {
            PermissionPolicyRole administratorRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "Administrators"));
            if(administratorRole == null) {
                administratorRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                administratorRole.Name = "Administrators";
                //Can access everything.
                administratorRole.IsAdministrative = true;
            }
            return administratorRole;
        }
        //Users can access and partially edit data (no create and delete capabilities) from their own department.
        private PermissionPolicyRole GetUserRole() {
            PermissionPolicyRole userRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "Users"));
            if(userRole == null) {
                userRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                userRole.Name = "Users";

                PermissionPolicyTypePermissionObject userTypePermission = ObjectSpace.CreateObject<PermissionPolicyTypePermissionObject>();
                userTypePermission.TargetType = typeof(Employee);
                userRole.TypePermissions.Add(userTypePermission);

                PermissionPolicyObjectPermissionsObject canViewEmployeesFromOwnDepartmentObjectPermission = ObjectSpace.CreateObject<PermissionPolicyObjectPermissionsObject>();
                canViewEmployeesFromOwnDepartmentObjectPermission.Criteria = "Department.Employees[Oid = CurrentUserId()]";
                //canViewEmployeesFromOwnDepartmentObjectPermission.Criteria = new ContainsOperator("Department.Employees", new BinaryOperator(new OperandProperty("Oid"), new FunctionOperator(CurrentUserIdOperator.OperatorName), BinaryOperatorType.Equal)).ToString();
                canViewEmployeesFromOwnDepartmentObjectPermission.NavigateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                canViewEmployeesFromOwnDepartmentObjectPermission.ReadState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                userTypePermission.ObjectPermissions.Add(canViewEmployeesFromOwnDepartmentObjectPermission);

                PermissionPolicyMemberPermissionsObject canEditOwnUserMemberPermission = ObjectSpace.CreateObject<PermissionPolicyMemberPermissionsObject>();
                canEditOwnUserMemberPermission.Members = "ChangePasswordOnFirstLogon; StoredPassword; FirstName; LastName;";
                canEditOwnUserMemberPermission.Criteria = "Oid = CurrentUserId()";
                //canEditOwnUserMemberPermission.Criteria = (new OperandProperty("Oid") == new FunctionOperator(CurrentUserIdOperator.OperatorName)).ToString();
                canEditOwnUserMemberPermission.WriteState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                userTypePermission.MemberPermissions.Add(canEditOwnUserMemberPermission);

                PermissionPolicyMemberPermissionsObject canEditUserAssociationsFromOwnDepartmentMemberPermission = ObjectSpace.CreateObject<PermissionPolicyMemberPermissionsObject>();
                canEditUserAssociationsFromOwnDepartmentMemberPermission.Members = "Tasks; Department;";
                canEditUserAssociationsFromOwnDepartmentMemberPermission.Criteria = "Department.Employees[Oid = CurrentUserId()]";
                //canEditUserAssociationsFromOwnDepartmentMemberPermission.Criteria = new ContainsOperator("Department.Employees", new BinaryOperator(new OperandProperty("Oid"), new FunctionOperator(CurrentUserIdOperator.OperatorName), BinaryOperatorType.Equal)).ToString();
                canEditUserAssociationsFromOwnDepartmentMemberPermission.WriteState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                userTypePermission.MemberPermissions.Add(canEditUserAssociationsFromOwnDepartmentMemberPermission);


                PermissionPolicyTypePermissionObject roleTypePermission = ObjectSpace.CreateObject<PermissionPolicyTypePermissionObject>();
                roleTypePermission.TargetType = typeof(PermissionPolicyRole);
                roleTypePermission.ReadState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                userRole.TypePermissions.Add(roleTypePermission);


                PermissionPolicyTypePermissionObject taskTypePermission = ObjectSpace.CreateObject<PermissionPolicyTypePermissionObject>();
                taskTypePermission.TargetType = typeof(EmployeeTask);
                taskTypePermission.NavigateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                userRole.TypePermissions.Add(taskTypePermission);

                PermissionPolicyMemberPermissionsObject canEditTaskAssociationsMemberPermission = ObjectSpace.CreateObject<PermissionPolicyMemberPermissionsObject>();
                canEditTaskAssociationsMemberPermission.Members = "AssignedTo;";
                canEditTaskAssociationsMemberPermission.Criteria = "AssignedTo.Department.Employees[Oid = CurrentUserId()]";
                //canEditTaskAssociationsMemberPermission.Criteria = new ContainsOperator("AssignedTo.Department.Oid", new BinaryOperator(new OperandProperty("Oid"), new FunctionOperator(CurrentUserIdOperator.OperatorName), BinaryOperatorType.Equal)).ToString();
                canEditTaskAssociationsMemberPermission.WriteState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                taskTypePermission.MemberPermissions.Add(canEditTaskAssociationsMemberPermission);

                PermissionPolicyObjectPermissionsObject canEditTasksFromOwnDepartmentObjectPermission = ObjectSpace.CreateObject<PermissionPolicyObjectPermissionsObject>();
                canEditTasksFromOwnDepartmentObjectPermission.Criteria = "AssignedTo.Department.Employees[Oid = CurrentUserId()]";
                //canyEditTasksFromOwnDepartmentObjectPermission.Criteria = new ContainsOperator("AssignedTo.Department.Oid", new BinaryOperator(new OperandProperty("Oid"), new FunctionOperator(CurrentUserIdOperator.OperatorName), BinaryOperatorType.Equal)).ToString();
                canEditTasksFromOwnDepartmentObjectPermission.NavigateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                canEditTasksFromOwnDepartmentObjectPermission.WriteState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                canEditTasksFromOwnDepartmentObjectPermission.ReadState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                taskTypePermission.ObjectPermissions.Add(canEditTasksFromOwnDepartmentObjectPermission);

                PermissionPolicyTypePermissionObject departmentTypePermission = ObjectSpace.CreateObject<PermissionPolicyTypePermissionObject>();
                departmentTypePermission.TargetType = typeof(Department);
                userRole.TypePermissions.Add(departmentTypePermission);

                PermissionPolicyObjectPermissionsObject canViewOwnDepartmentObjectPermission = ObjectSpace.CreateObject<PermissionPolicyObjectPermissionsObject>();
                canViewOwnDepartmentObjectPermission.Criteria = "Employees[Oid = CurrentUserId()]";
                //canViewOwnDepartmentObjectPermission.Criteria = new ContainsOperator("Employees", (new OperandProperty("Oid") == new FunctionOperator(CurrentUserIdOperator.OperatorName))).ToString();
                canViewOwnDepartmentObjectPermission.NavigateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                canViewOwnDepartmentObjectPermission.ReadState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                canViewOwnDepartmentObjectPermission.Save();
                departmentTypePermission.ObjectPermissions.Add(canViewOwnDepartmentObjectPermission);

                PermissionPolicyMemberPermissionsObject canEditAssociationsMemberPermission = ObjectSpace.CreateObject<PermissionPolicyMemberPermissionsObject>();
                canEditAssociationsMemberPermission.Members = "Employees;";
                canEditAssociationsMemberPermission.Criteria = "Employees[Oid = CurrentUserId()]";
                //canEditAssociationsMemberPermission.Criteria = new ContainsOperator("Employees", (new OperandProperty("Oid") == new FunctionOperator(CurrentUserIdOperator.OperatorName))).ToString();
                canEditAssociationsMemberPermission.WriteState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                departmentTypePermission.MemberPermissions.Add(canEditAssociationsMemberPermission);
            }
            return userRole;
        }
        //Managers can access and fully edit (including create and delete capabilities) data from their own department. However, they cannot access data from other departments.
        private PermissionPolicyRole GetManagerRole() {
            PermissionPolicyRole managerRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "Managers"));
            if(managerRole == null) {
                managerRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                managerRole.Name = "Managers";

                PermissionPolicyTypePermissionObject departmentTypePermission = ObjectSpace.CreateObject<PermissionPolicyTypePermissionObject>();
                departmentTypePermission.TargetType = typeof(Department);
                departmentTypePermission.NavigateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                managerRole.TypePermissions.Add(departmentTypePermission);

                PermissionPolicyObjectPermissionsObject canEditOwnDepartmentObjectPermission = ObjectSpace.CreateObject<PermissionPolicyObjectPermissionsObject>();
                canEditOwnDepartmentObjectPermission.Criteria = "Employees[Oid = CurrentUserId()]";
                //canEditOwnDepartmentObjectPermission.Criteria = new new ContainsOperator("Employees", (new OperandProperty("Oid") == new FunctionOperator(CurrentUserIdOperator.OperatorName))).ToString();
                canEditOwnDepartmentObjectPermission.NavigateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                canEditOwnDepartmentObjectPermission.ReadState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                canEditOwnDepartmentObjectPermission.WriteState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                canEditOwnDepartmentObjectPermission.DeleteState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                canEditOwnDepartmentObjectPermission.Save();
                departmentTypePermission.ObjectPermissions.Add(canEditOwnDepartmentObjectPermission);

                PermissionPolicyTypePermissionObject employeeTypePermission = ObjectSpace.CreateObject<PermissionPolicyTypePermissionObject>();
                employeeTypePermission.TargetType = typeof(Employee);
                employeeTypePermission.NavigateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                employeeTypePermission.CreateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                managerRole.TypePermissions.Add(employeeTypePermission);
                PermissionPolicyObjectPermissionsObject canEditEmployeesFromOwnDepartmentObjectPermission = ObjectSpace.CreateObject<PermissionPolicyObjectPermissionsObject>();
                canEditEmployeesFromOwnDepartmentObjectPermission.Criteria = "IsNull(Department) || Department.Employees[Oid = CurrentUserId()]";
                //canEditEmployeesFromOwnDepartmentObjectPermission.Criteria = (new NullOperator(new OperandProperty("Department")) | new ContainsOperator("Department.Employees", (new OperandProperty("Oid") == new FunctionOperator(CurrentUserIdOperator.OperatorName)))).ToString();
                canEditEmployeesFromOwnDepartmentObjectPermission.WriteState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                canEditEmployeesFromOwnDepartmentObjectPermission.DeleteState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                canEditEmployeesFromOwnDepartmentObjectPermission.NavigateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                canEditEmployeesFromOwnDepartmentObjectPermission.ReadState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                canEditEmployeesFromOwnDepartmentObjectPermission.Save();
                employeeTypePermission.ObjectPermissions.Add(canEditEmployeesFromOwnDepartmentObjectPermission);

                PermissionPolicyTypePermissionObject taskTypePermission = ObjectSpace.CreateObject<PermissionPolicyTypePermissionObject>();
                taskTypePermission.TargetType = typeof(EmployeeTask);
                taskTypePermission.NavigateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                taskTypePermission.CreateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                managerRole.TypePermissions.Add(taskTypePermission);
                PermissionPolicyObjectPermissionsObject canEditTasksOnlyFromOwnDepartmentObjectPermission = ObjectSpace.CreateObject<PermissionPolicyObjectPermissionsObject>();
                canEditTasksOnlyFromOwnDepartmentObjectPermission.Criteria = "IsNull(AssignedTo) || IsNull(AssignedTo.Department) || AssignedTo.Department.Employees[Oid = CurrentUserId()]";
                //canEditTasksOnlyFromOwnDepartmentObjectPermission.Criteria = (new NullOperator(new OperandProperty("AssignedTo")) | new NullOperator(new OperandProperty("AssignedTo.Department")) | new ContainsOperator("Department.Employees", (new OperandProperty("Oid") == new FunctionOperator(CurrentUserIdOperator.OperatorName)))).ToString();
                canEditTasksOnlyFromOwnDepartmentObjectPermission.NavigateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                canEditTasksOnlyFromOwnDepartmentObjectPermission.ReadState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                canEditTasksOnlyFromOwnDepartmentObjectPermission.WriteState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                canEditTasksOnlyFromOwnDepartmentObjectPermission.DeleteState = DevExpress.Persistent.Base.SecurityPermissionState.Allow;
                canEditTasksOnlyFromOwnDepartmentObjectPermission.Save();
                taskTypePermission.ObjectPermissions.Add(canEditTasksOnlyFromOwnDepartmentObjectPermission);
            }
            return managerRole;
        }
    }
}
