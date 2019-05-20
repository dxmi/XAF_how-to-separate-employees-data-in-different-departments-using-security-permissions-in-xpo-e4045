Imports System
Imports DevExpress.ExpressApp
Imports DevExpress.Data.Filtering
Imports MainDemo.[Module].BusinessObjects
Imports DevExpress.ExpressApp.SystemModule
Imports DevExpress.Persistent.Base.General
Imports DevExpress.Persistent.BaseImpl.PermissionPolicy

Namespace MainDemo.[Module].DatabaseUpdate
    Public Class Updater
        Inherits DevExpress.ExpressApp.Updating.ModuleUpdater

        Public Sub New(ByVal objectSpace As IObjectSpace, ByVal currentDBVersion As Version)
            MyBase.New(objectSpace, currentDBVersion)
        End Sub

        Public Overrides Sub UpdateDatabaseAfterUpdateSchema()
            MyBase.UpdateDatabaseAfterUpdateSchema()
            Dim devDepartment As Department = ObjectSpace.FindObject(Of Department)(CriteriaOperator.Parse("Title == 'R&D'"))

            If devDepartment Is Nothing Then
                devDepartment = ObjectSpace.CreateObject(Of Department)()
                devDepartment.Title = "R&D"
                devDepartment.Office = "1"
                devDepartment.Save()
            End If

            Dim supDepartment As Department = ObjectSpace.FindObject(Of Department)(CriteriaOperator.Parse("Title == 'Technical Support'"))

            If supDepartment Is Nothing Then
                supDepartment = ObjectSpace.CreateObject(Of Department)()
                supDepartment.Title = "Technical Support"
                supDepartment.Office = "2"
                supDepartment.Save()
            End If

            Dim mngDepartment As Department = ObjectSpace.FindObject(Of Department)(CriteriaOperator.Parse("Title == 'Management'"))

            If mngDepartment Is Nothing Then
                mngDepartment = ObjectSpace.CreateObject(Of Department)()
                mngDepartment.Title = "Management"
                mngDepartment.Office = "3"
                mngDepartment.Save()
            End If

            Dim administrator As Employee = ObjectSpace.FindObject(Of Employee)(CriteriaOperator.Parse("UserName == 'Admin'"))

            If administrator Is Nothing Then
                administrator = ObjectSpace.CreateObject(Of Employee)()
                administrator.UserName = "Admin"
                administrator.FirstName = "Admin"
                administrator.LastName = "Admin"
                administrator.Department = mngDepartment
                administrator.IsActive = True
                administrator.SetPassword("")
                administrator.Roles.Add(GetAdministratorRole())
                administrator.Save()
            End If

            Dim managerSam As Employee = ObjectSpace.FindObject(Of Employee)(CriteriaOperator.Parse("UserName == 'Sam'"))

            If managerSam Is Nothing Then
                managerSam = ObjectSpace.CreateObject(Of Employee)()
                managerSam.UserName = "Sam"
                managerSam.FirstName = "Sam"
                managerSam.LastName = "Jackson"
                managerSam.IsActive = True
                managerSam.SetPassword("")
                managerSam.Department = devDepartment
                managerSam.Roles.Add(GetManagerRole())
                managerSam.Roles.Add(GetUserRole())
                managerSam.Save()
            End If

            Dim userJohn As Employee = ObjectSpace.FindObject(Of Employee)(CriteriaOperator.Parse("UserName == 'John'"))

            If userJohn Is Nothing Then
                userJohn = ObjectSpace.CreateObject(Of Employee)()
                userJohn.UserName = "John"
                userJohn.FirstName = "John"
                userJohn.LastName = "Doe"
                userJohn.IsActive = True
                userJohn.SetPassword("")
                userJohn.Department = devDepartment
                userJohn.Roles.Add(GetUserRole())
                userJohn.Save()
            End If

            Dim managerMary As Employee = ObjectSpace.FindObject(Of Employee)(CriteriaOperator.Parse("UserName == 'Mary'"))

            If managerMary Is Nothing Then
                managerMary = ObjectSpace.CreateObject(Of Employee)()
                managerMary.UserName = "Mary"
                managerMary.FirstName = "Mary"
                managerMary.LastName = "Tellinson"
                managerMary.IsActive = True
                managerMary.SetPassword("")
                managerMary.Department = supDepartment
                managerMary.Roles.Add(GetManagerRole())
                managerMary.Roles.Add(GetUserRole())
                managerMary.Save()
            End If

            Dim userJoe As Employee = ObjectSpace.FindObject(Of Employee)(CriteriaOperator.Parse("UserName == 'Joe'"))

            If userJoe Is Nothing Then
                userJoe = ObjectSpace.CreateObject(Of Employee)()
                userJoe.UserName = "Joe"
                userJoe.FirstName = "Joe"
                userJoe.LastName = "Pitt"
                userJoe.IsActive = True
                userJoe.SetPassword("")
                userJoe.Department = supDepartment
                userJoe.Roles.Add(GetUserRole())
                userJoe.Save()
            End If

            If ObjectSpace.FindObject(Of EmployeeTask)(CriteriaOperator.Parse("Subject == 'Do homework'")) Is Nothing Then
                Dim task As EmployeeTask = ObjectSpace.CreateObject(Of EmployeeTask)()
                task.Subject = "Do homework"
                task.AssignedTo = managerSam
                task.DueDate = DateTime.Now
                task.Status = TaskStatus.NotStarted
                task.Description = "This is a task for Sam"
                task.Save()
            End If

            If ObjectSpace.FindObject(Of EmployeeTask)(CriteriaOperator.Parse("Subject == 'Prepare coffee for everyone'")) Is Nothing Then
                Dim task As EmployeeTask = ObjectSpace.CreateObject(Of EmployeeTask)()
                task.Subject = "Prepare coffee for everyone"
                task.AssignedTo = userJohn
                task.DueDate = DateTime.Now
                task.Status = TaskStatus.InProgress
                task.Description = "This is a task for John"
                task.Save()
            End If

            If ObjectSpace.FindObject(Of EmployeeTask)(CriteriaOperator.Parse("Subject == 'Read latest news'")) Is Nothing Then
                Dim task As EmployeeTask = ObjectSpace.CreateObject(Of EmployeeTask)()
                task.Subject = "Read latest news"
                task.AssignedTo = managerMary
                task.DueDate = DateTime.Now
                task.Status = TaskStatus.Completed
                task.Description = "This is a task for Mary"
                task.Save()
            End If

            If ObjectSpace.FindObject(Of EmployeeTask)(CriteriaOperator.Parse("Subject == 'Book tickets'")) Is Nothing Then
                Dim task As EmployeeTask = ObjectSpace.CreateObject(Of EmployeeTask)()
                task.Subject = "Book tickets"
                task.AssignedTo = userJoe
                task.DueDate = DateTime.Now
                task.Status = TaskStatus.Deferred
                task.Description = "This is a task for Joe"
                task.Save()
            End If

            ObjectSpace.CommitChanges()
        End Sub

        Private Function GetAdministratorRole() As PermissionPolicyRole
            Dim administratorRole As PermissionPolicyRole = ObjectSpace.FindObject(Of PermissionPolicyRole)(New BinaryOperator("Name", "Administrators"))

            If administratorRole Is Nothing Then
                administratorRole = ObjectSpace.CreateObject(Of PermissionPolicyRole)()
                administratorRole.Name = "Administrators"
                administratorRole.IsAdministrative = True
            End If

            Return administratorRole
        End Function

        Private Function GetUserRole() As PermissionPolicyRole
            Dim userRole As PermissionPolicyRole = ObjectSpace.FindObject(Of PermissionPolicyRole)(New BinaryOperator("Name", "Users"))

            If userRole Is Nothing Then
                userRole = ObjectSpace.CreateObject(Of PermissionPolicyRole)()
                userRole.Name = "Users"
                Dim userTypePermission As PermissionPolicyTypePermissionObject = ObjectSpace.CreateObject(Of PermissionPolicyTypePermissionObject)()
                userTypePermission.TargetType = GetType(Employee)
                userRole.TypePermissions.Add(userTypePermission)
                Dim canViewEmployeesFromOwnDepartmentObjectPermission As PermissionPolicyObjectPermissionsObject = ObjectSpace.CreateObject(Of PermissionPolicyObjectPermissionsObject)()
                canViewEmployeesFromOwnDepartmentObjectPermission.Criteria = "Department.Employees[Oid = CurrentUserId()]"
                canViewEmployeesFromOwnDepartmentObjectPermission.NavigateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                canViewEmployeesFromOwnDepartmentObjectPermission.ReadState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                userTypePermission.ObjectPermissions.Add(canViewEmployeesFromOwnDepartmentObjectPermission)
                Dim canEditOwnUserMemberPermission As PermissionPolicyMemberPermissionsObject = ObjectSpace.CreateObject(Of PermissionPolicyMemberPermissionsObject)()
                canEditOwnUserMemberPermission.Members = "ChangePasswordOnFirstLogon; StoredPassword; FirstName; LastName;"
                canEditOwnUserMemberPermission.Criteria = "Oid = CurrentUserId()"
                canEditOwnUserMemberPermission.WriteState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                userTypePermission.MemberPermissions.Add(canEditOwnUserMemberPermission)
                Dim canEditUserAssociationsFromOwnDepartmentMemberPermission As PermissionPolicyMemberPermissionsObject = ObjectSpace.CreateObject(Of PermissionPolicyMemberPermissionsObject)()
                canEditUserAssociationsFromOwnDepartmentMemberPermission.Members = "Tasks; Department;"
                canEditUserAssociationsFromOwnDepartmentMemberPermission.Criteria = "Department.Employees[Oid = CurrentUserId()]"
                canEditUserAssociationsFromOwnDepartmentMemberPermission.WriteState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                userTypePermission.MemberPermissions.Add(canEditUserAssociationsFromOwnDepartmentMemberPermission)
                Dim roleTypePermission As PermissionPolicyTypePermissionObject = ObjectSpace.CreateObject(Of PermissionPolicyTypePermissionObject)()
                roleTypePermission.TargetType = GetType(PermissionPolicyRole)
                roleTypePermission.ReadState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                userRole.TypePermissions.Add(roleTypePermission)
                Dim taskTypePermission As PermissionPolicyTypePermissionObject = ObjectSpace.CreateObject(Of PermissionPolicyTypePermissionObject)()
                taskTypePermission.TargetType = GetType(EmployeeTask)
                taskTypePermission.NavigateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                userRole.TypePermissions.Add(taskTypePermission)
                Dim canEditTaskAssociationsMemberPermission As PermissionPolicyMemberPermissionsObject = ObjectSpace.CreateObject(Of PermissionPolicyMemberPermissionsObject)()
                canEditTaskAssociationsMemberPermission.Members = "AssignedTo;"
                canEditTaskAssociationsMemberPermission.Criteria = "AssignedTo.Department.Employees[Oid = CurrentUserId()]"
                canEditTaskAssociationsMemberPermission.WriteState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                taskTypePermission.MemberPermissions.Add(canEditTaskAssociationsMemberPermission)
                Dim canEditTasksFromOwnDepartmentObjectPermission As PermissionPolicyObjectPermissionsObject = ObjectSpace.CreateObject(Of PermissionPolicyObjectPermissionsObject)()
                canEditTasksFromOwnDepartmentObjectPermission.Criteria = "AssignedTo.Department.Employees[Oid = CurrentUserId()]"
                canEditTasksFromOwnDepartmentObjectPermission.NavigateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                canEditTasksFromOwnDepartmentObjectPermission.WriteState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                canEditTasksFromOwnDepartmentObjectPermission.ReadState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                taskTypePermission.ObjectPermissions.Add(canEditTasksFromOwnDepartmentObjectPermission)
                Dim departmentTypePermission As PermissionPolicyTypePermissionObject = ObjectSpace.CreateObject(Of PermissionPolicyTypePermissionObject)()
                departmentTypePermission.TargetType = GetType(Department)
                userRole.TypePermissions.Add(departmentTypePermission)
                Dim canViewOwnDepartmentObjectPermission As PermissionPolicyObjectPermissionsObject = ObjectSpace.CreateObject(Of PermissionPolicyObjectPermissionsObject)()
                canViewOwnDepartmentObjectPermission.Criteria = "Employees[Oid = CurrentUserId()]"
                canViewOwnDepartmentObjectPermission.NavigateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                canViewOwnDepartmentObjectPermission.ReadState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                canViewOwnDepartmentObjectPermission.Save()
                departmentTypePermission.ObjectPermissions.Add(canViewOwnDepartmentObjectPermission)
                Dim canEditAssociationsMemberPermission As PermissionPolicyMemberPermissionsObject = ObjectSpace.CreateObject(Of PermissionPolicyMemberPermissionsObject)()
                canEditAssociationsMemberPermission.Members = "Employees;"
                canEditAssociationsMemberPermission.Criteria = "Employees[Oid = CurrentUserId()]"
                canEditAssociationsMemberPermission.WriteState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                departmentTypePermission.MemberPermissions.Add(canEditAssociationsMemberPermission)
            End If

            Return userRole
        End Function

        Private Function GetManagerRole() As PermissionPolicyRole
            Dim managerRole As PermissionPolicyRole = ObjectSpace.FindObject(Of PermissionPolicyRole)(New BinaryOperator("Name", "Managers"))

            If managerRole Is Nothing Then
                managerRole = ObjectSpace.CreateObject(Of PermissionPolicyRole)()
                managerRole.Name = "Managers"
                Dim departmentTypePermission As PermissionPolicyTypePermissionObject = ObjectSpace.CreateObject(Of PermissionPolicyTypePermissionObject)()
                departmentTypePermission.TargetType = GetType(Department)
                departmentTypePermission.NavigateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                managerRole.TypePermissions.Add(departmentTypePermission)
                Dim canEditOwnDepartmentObjectPermission As PermissionPolicyObjectPermissionsObject = ObjectSpace.CreateObject(Of PermissionPolicyObjectPermissionsObject)()
                canEditOwnDepartmentObjectPermission.Criteria = "Employees[Oid = CurrentUserId()]"
                canEditOwnDepartmentObjectPermission.NavigateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                canEditOwnDepartmentObjectPermission.ReadState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                canEditOwnDepartmentObjectPermission.WriteState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                canEditOwnDepartmentObjectPermission.DeleteState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                canEditOwnDepartmentObjectPermission.Save()
                departmentTypePermission.ObjectPermissions.Add(canEditOwnDepartmentObjectPermission)
                Dim employeeTypePermission As PermissionPolicyTypePermissionObject = ObjectSpace.CreateObject(Of PermissionPolicyTypePermissionObject)()
                employeeTypePermission.TargetType = GetType(Employee)
                employeeTypePermission.NavigateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                employeeTypePermission.CreateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                managerRole.TypePermissions.Add(employeeTypePermission)
                Dim canEditEmployeesFromOwnDepartmentObjectPermission As PermissionPolicyObjectPermissionsObject = ObjectSpace.CreateObject(Of PermissionPolicyObjectPermissionsObject)()
                canEditEmployeesFromOwnDepartmentObjectPermission.Criteria = "IsNull(Department) || Department.Employees[Oid = CurrentUserId()]"
                canEditEmployeesFromOwnDepartmentObjectPermission.WriteState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                canEditEmployeesFromOwnDepartmentObjectPermission.DeleteState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                canEditEmployeesFromOwnDepartmentObjectPermission.NavigateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                canEditEmployeesFromOwnDepartmentObjectPermission.ReadState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                canEditEmployeesFromOwnDepartmentObjectPermission.Save()
                employeeTypePermission.ObjectPermissions.Add(canEditEmployeesFromOwnDepartmentObjectPermission)
                Dim taskTypePermission As PermissionPolicyTypePermissionObject = ObjectSpace.CreateObject(Of PermissionPolicyTypePermissionObject)()
                taskTypePermission.TargetType = GetType(EmployeeTask)
                taskTypePermission.NavigateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                taskTypePermission.CreateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                managerRole.TypePermissions.Add(taskTypePermission)
                Dim canEditTasksOnlyFromOwnDepartmentObjectPermission As PermissionPolicyObjectPermissionsObject = ObjectSpace.CreateObject(Of PermissionPolicyObjectPermissionsObject)()
                canEditTasksOnlyFromOwnDepartmentObjectPermission.Criteria = "IsNull(AssignedTo) || IsNull(AssignedTo.Department) || AssignedTo.Department.Employees[Oid = CurrentUserId()]"
                canEditTasksOnlyFromOwnDepartmentObjectPermission.NavigateState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                canEditTasksOnlyFromOwnDepartmentObjectPermission.ReadState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                canEditTasksOnlyFromOwnDepartmentObjectPermission.WriteState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                canEditTasksOnlyFromOwnDepartmentObjectPermission.DeleteState = DevExpress.Persistent.Base.SecurityPermissionState.Allow
                canEditTasksOnlyFromOwnDepartmentObjectPermission.Save()
                taskTypePermission.ObjectPermissions.Add(canEditTasksOnlyFromOwnDepartmentObjectPermission)
            End If

            Return managerRole
        End Function
    End Class
End Namespace
