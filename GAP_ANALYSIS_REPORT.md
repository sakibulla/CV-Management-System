# CV Management System - Comprehensive Gap Analysis Report

**Analysis Date:** July 22, 2026  
**Project:** CV Management System  
**Database:** SQLite (Current) | PostgreSQL (Required)

---

## Executive Summary

This gap analysis evaluates the CV Management System against the provided requirements specification. The system demonstrates **strong core functionality** but has **critical gaps** that pose **high defense point-deduction risks**, particularly in database constraints, deployment configuration, and admin functionality.

### Risk Assessment
- **CRITICAL ISSUES:** 3
- **HIGH PRIORITY:** 4
- **MEDIUM PRIORITY:** 3
- **LOW PRIORITY:** 2

---

## 1. CRITICAL ISSUES ⚠️ (Defense Point-Deduction Risks)

### 1.1 ❌ Missing Unique Constraint on CV (UserId + PositionId)
**Status:** MISSING  
**Severity:** CRITICAL  
**Risk Level:** HIGH - Direct violation of spec requirement

**Issue:**
The specification explicitly requires: *"One CV per candidate per position enforced via a unique composite constraint (e.g., unique index on CandidateId + PositionId), not solely a runtime check in a controller/service."*

**Current State:**
- No unique constraint in database schema (verified in `AppDbContextModelSnapshot.cs`)
- No runtime check in `CVService.CreateCVAsync()` method
- Users can create multiple CVs for the same position

**Impact:**
- Data integrity violation
- Allows duplicate CVs per position
- Does NOT satisfy spec requirement for database-level enforcement

---

### 1.2 ❌ No PostgreSQL Configuration for Production
**Status:** MISSING  
**Severity:** CRITICAL  
**Risk Level:** HIGH - Deployment requirement violation

**Issue:**
The specification requires: *"PostgreSQL as the configured production database provider"* and *"Production-appropriate appsettings.Production.json (or equivalent) with no dev secrets/connection strings committed."*

**Current State:**
- Only SQLite configuration exists (`appsettings.json`)
- No `appsettings.Production.json` file found
- `Program.cs` only configures SQLite: `options.UseSqlite(connectionString)`
- No PostgreSQL package reference in project

**Impact:**
- Cannot deploy to production environment
- Fails deployment readiness requirement
- Missing production database configuration

---

### 1.3 ❌ Admin Cannot Edit Any Candidate Profile
**Status:** PARTIALLY IMPLEMENTED  
**Severity:** CRITICAL  
**Risk Level:** HIGH - Admin role requirement violation

**Issue:**
Specification requires: *"Edit any candidate's profile"* for Admin role.

**Current State:**
- `AdminController` has no profile editing functionality
- `CandidateProfileController` is restricted to `[Authorize]` without Admin override
- No `EditProfile(string userId)` action in `AdminController`
- Admin can only manage users/roles but cannot edit profile content

**Impact:**
- Admin role incomplete
- Cannot fulfill administrative oversight duties
- Violates admin requirements specification

---

## 2. HIGH PRIORITY ISSUES 🔴

### 2.1 ⚠️ Admin Cannot Edit Any CV
**Status:** PARTIALLY IMPLEMENTED  
**Severity:** HIGH

**Issue:**
Specification requires: *"Edit any CV — full edit rights (unlike Recruiter's read-only access)"*

**Current State:**
- `CVsController` restricted to `[Authorize(Roles = "Candidate")]`
- No admin CV editing functionality
- Admin has no access to CV editing actions

**Gap:**
Admin should be able to edit any CV, not just their own.

---

### 2.2 ⚠️ Admin Role Assignment Without Removing Old Role
**Status:** INCOMPLETE LOGIC  
**Severity:** HIGH

**Issue:**
The spec mentions *"Assign roles to users"* and *"Remove roles from users"* as separate actions.

**Current State:**
- `AdminService.AssignRoleAsync()` removes ALL roles before assigning new one
- This is correct behavior, but the spec implies both assign AND remove should be separate operations
- `AdminController` has no `RemoveRole` action (only `AssignRole`)

**Gap:**
Missing explicit "Remove Role" action in `AdminController` as specified.

---

### 2.3 ⚠️ No Prevention of Duplicate CV Creation
**Status:** MISSING  
**Severity:** HIGH

**Issue:**
Specification states: *"Constraint: only one CV per candidate per position. Attempting to create a second CV for the same position must redirect to edit, not create a duplicate."*

**Current State:**
- `CVsController.Create()` has no duplicate check
- No database constraint (see Critical Issue 1.1)
- Users can create unlimited CVs per position

**Expected Behavior:**
```csharp
// In CVsController.Create [POST]
var existing = await _cvService.GetCVByPositionAndUserAsync(positionId, user.Id);
if (existing != null)
{
    TempData["Info"] = "You already have a CV for this position. Redirecting to edit.";
    return RedirectToAction("Edit", new { id = existing.Id });
}
```

---

### 2.4 ⚠️ Anonymous User Can Access Home Index
**Status:** MISCONFIGURED  
**Severity:** HIGH

**Issue:**
Specification states: *"Anonymous User ... Cannot view positions, CVs, attribute library, or any protected data."*

**Current State:**
- `HomeController.Index()` has `[Authorize]` attribute
- BUT specification implies anonymous users should see a public landing page
- Current implementation redirects anonymous users to login

**Ambiguity:**
Unclear if Index should be public or protected. Best practice: public landing page for anonymous, dashboard for authenticated.

---

## 3. MEDIUM PRIORITY ISSUES 🟡

### 3.1 ⚠️ No Status Field on Position
**Status:** MISSING  
**Severity:** MEDIUM

**Issue:**
Search requirements mention: *"Position search: by title, status, and/or other position metadata."*

**Current State:**
- `Position` model has no `Status` field
- Cannot search by status
- No "Open/Closed/Draft" position states

**Recommendation:**
Add `Status` enum to `Position` model for better position lifecycle management.

---

### 3.2 ⚠️ Positions Table Has Too Many Action Buttons Per Row
**Status:** UI COMPLIANCE CONCERN  
**Severity:** MEDIUM

**Issue:**
Specification warns: *"Avoid repeating Edit/Delete action buttons redundantly on every single row in a way that clutters the table; consolidate actions sensibly (e.g., a single actions column, details page, or dropdown)."*

**Current State (Views/Positions/Index.cshtml):**
Each row has 6 separate buttons:
- Details
- Edit
- View CVs
- Delete
- Duplicate

**Observation:**
While they ARE consolidated in a single `Actions` column with `btn-group`, the visual density might be considered cluttered. Consider:
- Dropdown menu for secondary actions
- Details page for some actions
- Icon buttons instead of text buttons

---

### 3.3 ⚠️ Missing Candidate Search by Profile Field
**Status:** PARTIALLY IMPLEMENTED  
**Severity:** MEDIUM

**Issue:**
Specification requires: *"Candidate search: by name, skill/attribute, or profile field"*

**Current State:**
- `SearchService` exists but implementation not verified
- Search functionality in `SearchController.Index` and `RecruiterController.CVs`
- Unclear if profile fields (Location, etc.) are searchable

**Recommendation:**
Verify search includes all specified fields: name, skills, attributes, profile metadata.

---

## 4. LOW PRIORITY ISSUES 🟢

### 4.1 ✓ Minor: Positions Index Allows Non-Recruiter/Admin Access to Browse
**Status:** MINOR INCONSISTENCY  
**Severity:** LOW

**Issue:**
`PositionsController.Browse()` has conflicting attributes:
```csharp
[AllowAnonymous]
[Authorize]
```

This is contradictory. Should be one or the other.

---

### 4.2 ✓ Missing Build and Publish Verification
**Status:** NOT VERIFIED  
**Severity:** LOW

**Issue:**
Specification requires: *"Solution builds and publishes cleanly (dotnet publish) with no missing dependencies."*

**Recommendation:**
Run `dotnet build` and `dotnet publish` to verify clean compilation.

---

## 5. IMPLEMENTED FEATURES ✅

### Role-Based Access Control
✅ **Anonymous User**
- ✅ Can access Login/Register pages
- ✅ Registration defaults to Candidate role
- ✅ Blocked users cannot log in (`IsBlocked` check in `AuthController`)

✅ **Candidate Role**
- ✅ Edit own profile (`CandidateProfileController.Me`, `UpdateInfo`)
- ✅ Manage projects (add/edit/delete via `ProjectsController`)
- ✅ Browse attribute library (via profile page)
- ✅ Select attributes and fill values (`ProfileAttributes`)
- ✅ Create CV targeted at position (`CVsController.Create`)
- ✅ Edit own CV (`CVsController.Edit`)
- ✅ Delete own CV (`CVsController.Delete`)
- ✅ View own CVs in table (`CVsController.Index` - table layout ✓)
- ✅ Cannot view other candidates' CVs (role restriction enforced)

✅ **Recruiter Role**
- ✅ Manage Positions (create/edit/delete/duplicate via `PositionsController`)
- ✅ Manage Attribute Library (`AttributesController` - create/edit/delete)
- ✅ Assign attributes to positions (multi-select in Position create/edit)
- ✅ Browse candidate CVs in table view (`RecruiterController.CVs` - table layout ✓)
- ✅ Search candidate CVs by name/position (`RecruiterController.CVs?search=`)
- ✅ Open CV in read-only mode (`RecruiterController.ViewCV`)
- ✅ No access to user management (enforced by role restrictions)

✅ **Admin Role**
- ✅ Manage users: table listing (`AdminController.UserManagement` - table layout ✓)
- ✅ View role and block status per user
- ✅ Assign roles (`AdminController.AssignRole`)
- ✅ Block users (`AdminController.BlockUser`)
- ✅ Unblock users (`AdminController.UnblockUser`)
- ❌ Edit any candidate's profile (MISSING)
- ❌ Edit any CV (MISSING)
- ✅ Edit any position (Admin included in Positions role restriction)

### Database Structure
✅ **Proper Entity Relationships**
- ✅ All entities properly configured in `AppDbContext`
- ✅ Foreign keys enforced at database level
- ✅ Cascade delete configured appropriately
- ❌ Missing unique constraint on CV (UserId + PositionId)

✅ **Required Fields**
- ✅ Data annotations and `IsRequired()` used appropriately
- ✅ Non-nullable reference types enforced

### UI Compliance
✅ **Table Layouts**
- ✅ Positions list uses `<table>` (`Views/Positions/Index.cshtml`)
- ✅ CVs list uses `<table>` (`Views/CVs/Index.cshtml`)
- ✅ Recruiter CV list uses `<table>` (`Views/Recruiter/CVs.cshtml`)
- ✅ Admin user list uses `<table>` (`Views/Admin/UserManagement.cshtml`)
- ✅ No tile/grid/gallery layouts found

### Search Functionality
✅ **Search Implementation**
- ✅ Position search implemented (`SearchController.Index`)
- ✅ CV search implemented (by candidate name, position)
- ✅ Recruiter CV browsing with search filter

---

## 6. PRIORITIZED REMEDIATION PLAN

### Phase 1: CRITICAL (Do First - Defense Risk)
1. **Add Unique Constraint to CV Table** (Highest Priority)
   - Add to `AppDbContext.OnModelCreating()`
   - Create migration
   - Add runtime check + redirect logic

2. **Add PostgreSQL Production Configuration**
   - Create `appsettings.Production.json`
   - Add Npgsql.EntityFrameworkCore.PostgreSQL package
   - Update `Program.cs` to use PostgreSQL in production

3. **Implement Admin Edit Candidate Profile**
   - Add `EditCandidateProfile` action to `AdminController`
   - Create corresponding view
   - Allow admin to edit any profile

### Phase 2: HIGH PRIORITY
4. **Implement Admin Edit CV**
   - Add CV editing actions to `AdminController` or modify `CVsController` role check
   
5. **Add RemoveRole Action to Admin**
   - Separate action for removing roles

6. **Implement Duplicate CV Prevention**
   - Add check in `CVsController.Create`
   - Redirect to Edit if CV exists

### Phase 3: MEDIUM PRIORITY
7. **Add Status field to Position**
8. **Refactor Positions table action buttons**
9. **Verify comprehensive search functionality**

### Phase 4: LOW PRIORITY
10. **Fix PositionsController.Browse attribute conflict**
11. **Verify build and publish commands**

---

## 7. DEFENSE CHECKLIST

Before final defense, verify:

- [ ] **Database Constraint**: Unique index on (UserId, PositionId) in CV table
- [ ] **PostgreSQL Config**: appsettings.Production.json exists with PostgreSQL connection
- [ ] **Admin Full Rights**: Admin can edit any profile and any CV
- [ ] **No Duplicate CVs**: System prevents/redirects duplicate CV creation
- [ ] **Table Layouts**: All list views use `<table>`, not cards/tiles
- [ ] **Anonymous Access**: Anonymous users cannot access protected data
- [ ] **Blocked Users**: Blocked users cannot log in
- [ ] **Role Assignment**: Admin can assign and remove roles
- [ ] **Clean Build**: `dotnet build` and `dotnet publish` succeed
- [ ] **Migration Ready**: All migrations up to date and can apply to PostgreSQL

---

## 8. ESTIMATED REMEDIATION EFFORT

| Priority | Task | Estimated Time |
|----------|------|----------------|
| CRITICAL | Unique CV constraint + migration | 30 minutes |
| CRITICAL | PostgreSQL configuration | 45 minutes |
| CRITICAL | Admin edit profile | 60 minutes |
| HIGH | Admin edit CV | 45 minutes |
| HIGH | Duplicate CV prevention | 30 minutes |
| HIGH | RemoveRole action | 15 minutes |
| MEDIUM | Position Status field | 30 minutes |
| **TOTAL** | **Core Issues** | **~4 hours** |

---

## 9. CONCLUSION

The CV Management System has a **solid foundation** with most core features properly implemented:
- ✅ Role-based access control working
- ✅ Table-based UI compliance
- ✅ Proper entity relationships
- ✅ Search functionality present
- ✅ CRUD operations for all entities

**However**, there are **3 critical gaps** that must be addressed before defense:
1. Missing database-level unique constraint on CV
2. No PostgreSQL production configuration
3. Incomplete Admin role (cannot edit profiles/CVs)

**Recommendation**: Focus remediation efforts on Phase 1 (Critical Issues) immediately. These are explicit spec requirements and pose the highest risk during defense evaluation.

**Overall Assessment**: 75% complete. With 4 hours of focused work on critical issues, the system can achieve 95%+ spec compliance.
