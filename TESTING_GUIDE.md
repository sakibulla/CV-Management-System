# Quick Testing Guide - Recruiter CV Browsing

## Setup
1. **Stop any running instances** of the application
2. **Build:** `dotnet build`
3. **Run:** `dotnet run` or press F5 in Visual Studio
4. **Access:** Navigate to `https://localhost:5001` or the appropriate URL

## Test User Credentials
Check `Data/SeedData.cs` for test user credentials, typically:
- **Recruiter:** recruiter@example.com / Password123!
- **Admin:** admin@example.com / Password123!  
- **Candidate:** candidate@example.com / Password123!

## Quick Test Checklist

### ✅ Navigation Menu (Recruiter)
- [ ] Login as Recruiter
- [ ] Verify "Browse CVs" link appears in navigation
- [ ] Verify menu shows: Positions, Attributes, Browse CVs

### ✅ Navigation Menu (Admin)
- [ ] Login as Admin
- [ ] Verify menu shows: Positions, Attributes, Browse CVs, User Management

### ✅ Main CV List Page
- [ ] Navigate to Browse CVs
- [ ] URL should be: `/Recruiter/CVs`
- [ ] Verify table displays all CVs with columns:
  - Candidate Name (+ email)
  - CV Title
  - Position (clickable)
  - Created Date
  - Last Updated
  - Actions (View Details button)

### ✅ Search Functionality
- [ ] Enter a candidate name in search box
- [ ] Click Search
- [ ] Verify results are filtered
- [ ] Click "Clear Search" to show all CVs again

### ✅ CV Details View
- [ ] Click "View Details" on any CV
- [ ] URL should be: `/Recruiter/ViewCV/{id}`
- [ ] Verify display shows:
  - "Read-Only View" badge
  - Candidate name, email, location
  - Position title (clickable) and description
  - Created and updated dates
  - CV Attributes section
  - Associated Projects section
- [ ] Verify NO edit or delete buttons exist
- [ ] Test "Back to All CVs" button
- [ ] Test "View Other CVs for this Position" button

### ✅ Position Integration
- [ ] Go to Positions page
- [ ] Click "View CVs" button on any position
- [ ] URL should be: `/Recruiter/PositionCVs/{positionId}`
- [ ] Verify only CVs for that position are shown
- [ ] Verify breadcrumb shows: Positions → [Position] → Candidate CVs

### ✅ Position Details Integration
- [ ] Go to Position Details page
- [ ] Click "View CVs for this Position" button
- [ ] Verify it navigates to filtered CV list

### ✅ Access Control
- [ ] Logout and login as Candidate
- [ ] Verify "Browse CVs" does NOT appear in menu
- [ ] Try accessing `/Recruiter/CVs` directly
- [ ] Should be denied (403 or redirect to access denied)

## Expected URLs

| Page | URL | Access |
|------|-----|--------|
| All CVs | `/Recruiter/CVs` | Recruiter, Admin |
| Search CVs | `/Recruiter/CVs?search=term` | Recruiter, Admin |
| CV Details | `/Recruiter/ViewCV/1` | Recruiter, Admin |
| Position CVs | `/Recruiter/PositionCVs/1` | Recruiter, Admin |

## Visual Checks

### Icons Should Appear
- 👤 Candidate icon (bi-person-circle)
- 📋 Attributes icon (bi-list-check)
- 📁 Projects icon (bi-folder)
- 👁 Eye icon on View buttons (bi-eye)
- ← Arrow on Back buttons (bi-arrow-left)
- 💼 Briefcase on position links (bi-briefcase)

### Styling Should Look Good
- Bootstrap cards with shadows
- Clean table layouts
- Hover effects on table rows
- Consistent button styling
- Breadcrumb navigation
- Badge indicators

## Common Issues & Solutions

### Issue: "Browse CVs" not showing in menu
**Solution:** Check that you're logged in as Recruiter or Admin

### Issue: Icons not showing (boxes instead)
**Solution:** Verify Bootstrap Icons CDN is loaded (check browser console for errors)

### Issue: "No CVs found"
**Solution:** Create some CVs as a Candidate user first

### Issue: Build fails with "file in use"
**Solution:** Stop the running application before building

### Issue: 403 Forbidden
**Solution:** You're not logged in as Recruiter or Admin

## Creating Test Data

To test the feature properly, you need some CVs:

1. **Login as Candidate**
2. **Go to Browse Positions**
3. **Click "Create CV for this Position"**
4. **Fill in the CV form and submit**
5. **Repeat for multiple positions**
6. **Logout and login as Recruiter**
7. **Now test the CV browsing features**

## Success Indicators

✅ Recruiters can see all CVs across all candidates  
✅ Search filters work correctly  
✅ CV details show all information in read-only mode  
✅ Position filtering works (only shows CVs for that position)  
✅ Navigation between pages works smoothly  
✅ Candidates cannot access recruiter CV browsing  
✅ No edit/delete buttons appear for recruiters  
✅ Admin has same access as recruiters  

## Report Issues

If you find any issues:
1. Note the URL where the issue occurs
2. Note your user role (Recruiter/Admin/Candidate)
3. Describe what you expected vs what happened
4. Check browser console for errors (F12)
5. Check application logs for server-side errors
