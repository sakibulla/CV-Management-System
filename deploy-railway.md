# Quick Deploy to Railway - Step by Step

## ⚡ Fastest Way to Deploy (5 minutes)

### Step 1: Sign Up for Railway
1. Go to https://railway.app
2. Click "Login" and select "Login with GitHub"
3. Authorize Railway to access your GitHub account

### Step 2: Create New Project
1. Click "New Project" button
2. Select "Deploy from GitHub repo"
3. Choose the repository: `sakibulla/CV-Management-System`
4. Railway will automatically detect the Dockerfile and start building

### Step 3: Add PostgreSQL Database
1. In your project dashboard, click "New" button
2. Select "Database"
3. Choose "Add PostgreSQL"
4. Railway will automatically:
   - Create a PostgreSQL database
   - Generate a connection string
   - Link it to your application

### Step 4: Configure Environment Variables (Automatic)
Railway automatically sets:
- `DATABASE_URL` - PostgreSQL connection string
- `PORT` - Application port

You need to add:
1. Click on your web service
2. Go to "Variables" tab
3. Add these variables:
   ```
   ASPNETCORE_ENVIRONMENT = Production
   ASPNETCORE_URLS = http://+:${{PORT}}
   ConnectionStrings__DefaultConnection = ${{DATABASE_URL}}
   ```

**Note**: Railway automatically injects `${{DATABASE_URL}}` from the PostgreSQL service.

### Step 5: Deploy!
1. Railway automatically deploys on every git push
2. Wait 2-3 minutes for the first deployment
3. Click "View Logs" to monitor progress
4. Once deployed, click the generated URL (e.g., `https://your-app.railway.app`)

### Step 6: Test Your Application
1. Visit your Railway URL
2. Login with default credentials:
   - **Admin**: admin@cvms.com / Admin@123
   - **Recruiter**: recruiter@cvms.com / Recruiter@123
   - **Candidate**: candidate@cvms.com / Candidate@123
3. **IMPORTANT**: Change all default passwords immediately!

---

## 🔄 Automatic Deployments

After initial setup:
- Every `git push` to main branch triggers automatic deployment
- Railway rebuilds and redeploys automatically
- Zero downtime deployments

---

## 📊 Monitor Your Application

In Railway dashboard:
- **Metrics**: View CPU, memory, and network usage
- **Logs**: Real-time application logs
- **Deployments**: See deployment history
- **Settings**: Configure custom domains, environment variables

---

## 🆓 Free Tier Limits

Railway free tier includes:
- 500 hours of usage per month
- $5 free credit monthly
- PostgreSQL database included
- SSL certificate included

---

## 🚀 Custom Domain (Optional)

1. Go to your service settings
2. Click "Networking" tab
3. Add your custom domain
4. Update DNS records as instructed
5. SSL is automatically provisioned

---

## ⚠️ Important: Change Default Passwords

After deployment:
1. Login as admin
2. Go to User Management
3. Change passwords for all default users
4. Or delete default users and create new ones

---

## 🔍 Troubleshooting

### Build Failed
- Check Dockerfile syntax
- View build logs in Railway
- Ensure all dependencies are listed in .csproj

### Database Connection Error
- Verify `ConnectionStrings__DefaultConnection` is set to `${{DATABASE_URL}}`
- Check PostgreSQL service is running
- Review application logs

### Application Won't Start
- Check environment variables
- View logs for startup errors
- Ensure port is set correctly: `http://+:${{PORT}}`

---

## 📝 Need Help?

- Railway Docs: https://docs.railway.app
- Railway Discord: https://discord.gg/railway
- GitHub Issues: Your repository issues page

---

**That's it! Your application should now be live! 🎉**
