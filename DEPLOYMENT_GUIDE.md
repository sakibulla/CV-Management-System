# CV Management System - Deployment Guide

This guide covers multiple deployment options for the CV Management System.

## Quick Deployment Options

### 🚀 Option 1: Railway (Recommended for Quick Start)

Railway offers free tier with PostgreSQL database included.

#### Steps:

1. **Create Railway Account**
   - Go to [railway.app](https://railway.app)
   - Sign up with GitHub

2. **Deploy from GitHub**
   ```bash
   # Push your code to GitHub (already done)
   # Then in Railway dashboard:
   # - Click "New Project"
   # - Select "Deploy from GitHub repo"
   # - Choose your CV-Management-System repository
   ```

3. **Add PostgreSQL Database**
   - In your Railway project, click "New"
   - Select "Database" → "PostgreSQL"
   - Railway will automatically create and link the database

4. **Set Environment Variables**
   
   Railway should auto-detect most settings, but verify:
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ConnectionStrings__DefaultConnection=<auto-provided-by-railway>
   ```

5. **Deploy**
   - Railway auto-deploys on every git push
   - Your app will be available at: `https://your-app.railway.app`

**Pros**: Free tier, auto-deployments, PostgreSQL included, simple setup
**Cons**: Limited free tier hours per month

---

### 🎨 Option 2: Render

Render offers free tier with PostgreSQL support.

#### Steps:

1. **Create Render Account**
   - Go to [render.com](https://render.com)
   - Sign up with GitHub

2. **Create PostgreSQL Database**
   - Dashboard → "New" → "PostgreSQL"
   - Name: `cvms-db`
   - Free plan
   - Note the connection string

3. **Deploy Web Service**
   - Dashboard → "New" → "Web Service"
   - Connect your GitHub repository
   - Settings:
     - Name: `cv-management-system`
     - Runtime: `Docker`
     - Plan: `Free`

4. **Set Environment Variables**
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ASPNETCORE_URLS=http://+:10000
   ConnectionStrings__DefaultConnection=<your-postgres-connection-string>
   ```

5. **Deploy**
   - Click "Create Web Service"
   - Render will build and deploy automatically
   - App URL: `https://cv-management-system.onrender.com`

**Pros**: Free tier, good performance, easy to use
**Cons**: Spins down after inactivity (takes ~30s to wake up)

---

### ☁️ Option 3: Azure App Service

Azure provides enterprise-grade hosting with free tier.

#### Prerequisites:
- Install Azure CLI: `winget install Microsoft.AzureCLI`
- Azure account (free tier available)

#### Steps:

1. **Login to Azure**
   ```bash
   az login
   ```

2. **Create Resource Group**
   ```bash
   az group create --name CVMSResourceGroup --location eastus
   ```

3. **Create App Service Plan**
   ```bash
   az appservice plan create --name CVMSPlan --resource-group CVMSResourceGroup --sku F1 --is-linux
   ```

4. **Create PostgreSQL Database**
   ```bash
   az postgres flexible-server create \
     --resource-group CVMSResourceGroup \
     --name cvms-postgres-server \
     --location eastus \
     --admin-user cvmsadmin \
     --admin-password <YourSecurePassword> \
     --sku-name Standard_B1ms \
     --tier Burstable \
     --version 16
   ```

5. **Create Web App**
   ```bash
   az webapp create \
     --resource-group CVMSResourceGroup \
     --plan CVMSPlan \
     --name cv-management-system-app \
     --deployment-container-image-name mcr.microsoft.com/dotnet/aspnet:8.0
   ```

6. **Configure Connection String**
   ```bash
   az webapp config connection-string set \
     --resource-group CVMSResourceGroup \
     --name cv-management-system-app \
     --settings DefaultConnection="Host=cvms-postgres-server.postgres.database.azure.com;Database=CVManagementSystem;Username=cvmsadmin;Password=<YourSecurePassword>" \
     --connection-string-type PostgreSQL
   ```

7. **Deploy Application**
   ```bash
   cd c:\Projects\Itransition\CVManagementSystem
   dotnet publish -c Release -o ./publish
   cd publish
   zip -r ../deploy.zip .
   az webapp deployment source config-zip \
     --resource-group CVMSResourceGroup \
     --name cv-management-system-app \
     --src ../deploy.zip
   ```

**Pros**: Enterprise-grade, great integration with Microsoft services
**Cons**: More complex setup, limited free tier

---

### 🐳 Option 4: Docker (Self-Hosted or Any Platform)

Use Docker to deploy on your own server or any Docker-supporting platform.

#### Local Testing:

1. **Build and Run with Docker Compose**
   ```bash
   cd c:\Projects\Itransition\CVManagementSystem
   docker-compose up -d
   ```

2. **Access Application**
   - App: http://localhost:8080
   - PostgreSQL: localhost:5432

3. **View Logs**
   ```bash
   docker-compose logs -f web
   ```

4. **Stop Services**
   ```bash
   docker-compose down
   ```

#### Deploy to Any Docker Host:

1. **Build Image**
   ```bash
   docker build -t cvmanagement:latest .
   ```

2. **Tag for Registry** (Docker Hub example)
   ```bash
   docker tag cvmanagement:latest yourusername/cvmanagement:latest
   ```

3. **Push to Registry**
   ```bash
   docker push yourusername/cvmanagement:latest
   ```

4. **Deploy on Server**
   ```bash
   # On your server
   docker pull yourusername/cvmanagement:latest
   docker run -d -p 8080:8080 \
     -e ASPNETCORE_ENVIRONMENT=Production \
     -e ConnectionStrings__DefaultConnection="<your-postgres-connection>" \
     yourusername/cvmanagement:latest
   ```

**Pros**: Maximum flexibility, portable, works anywhere
**Cons**: Requires server management knowledge

---

### 🔧 Option 5: DigitalOcean App Platform

1. **Create Account** at [digitalocean.com](https://digitalocean.com)

2. **Create New App**
   - Apps → "Create App"
   - Choose GitHub repository
   - Select Dockerfile deployment

3. **Add Database**
   - Add Component → "Database"
   - Choose PostgreSQL
   - Dev or Basic plan

4. **Configure Environment**
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ConnectionStrings__DefaultConnection=${db.DATABASE_URL}
   ```

5. **Deploy**
   - Review and create
   - Auto-deploys on git push

**Pros**: Simple, includes monitoring, auto-scaling
**Cons**: No free tier (starts at $5/month)

---

## Environment Variables Reference

All platforms need these environment variables:

| Variable | Value | Description |
|----------|-------|-------------|
| `ASPNETCORE_ENVIRONMENT` | `Production` | Sets production mode |
| `ASPNETCORE_URLS` | `http://+:8080` | Port configuration |
| `ConnectionStrings__DefaultConnection` | `<postgres-connection>` | Database connection |

### PostgreSQL Connection String Format:
```
Host=<host>;Port=5432;Database=CVManagementSystem;Username=<user>;Password=<password>;SSL Mode=Require
```

---

## Post-Deployment Checklist

After deploying to any platform:

- ✅ **Test Default Login**
  - Admin: admin@cvms.com / Admin@123
  - Change password immediately!

- ✅ **Change Default Passwords**
  ```
  Login → Profile → Change Password
  ```

- ✅ **Verify Database Migrations**
  - Migrations run automatically on startup
  - Check logs for "Database migration completed"

- ✅ **Test Core Features**
  - Register new user
  - Create position (as Recruiter)
  - Create CV (as Candidate)
  - Search functionality

- ✅ **Configure HTTPS**
  - Most platforms provide SSL automatically
  - Verify https:// works

- ✅ **Set Up Monitoring**
  - Enable application logging
  - Set up alerts for errors

---

## Troubleshooting

### Database Connection Issues

**Error**: "Failed to connect to database"

**Solutions**:
1. Verify connection string format
2. Check if database allows connections from your host
3. Ensure firewall rules allow traffic
4. For Azure/Railway: Wait 2-3 minutes for database provisioning

### Migration Errors

**Error**: "Pending migrations detected"

**Solution**: Migrations run automatically, but you can manually run:
```bash
dotnet ef database update --connection "<your-connection-string>"
```

### Application Won't Start

**Checklist**:
- [ ] Check environment variables are set
- [ ] Verify connection string is correct
- [ ] Check application logs
- [ ] Ensure port 8080 is accessible
- [ ] Verify .NET 8.0 runtime is available

### Performance Issues

**Tips**:
1. Use connection pooling (enabled by default)
2. Enable response caching
3. Optimize database queries
4. Consider upgrading hosting tier

---

## Security Best Practices

Before going to production:

1. **Change Default Passwords**
   - Update all seeded user passwords
   - Or disable seed data in production

2. **Use Environment Variables**
   - Never commit connection strings
   - Store secrets in platform secret managers

3. **Enable HTTPS**
   - Force HTTPS redirects
   - Use HSTS headers

4. **Configure CORS** (if needed)
   - Restrict allowed origins
   - Don't use wildcard in production

5. **Update Connection String**
   - Use strong database password
   - Enable SSL for database connection

6. **Enable Logging**
   - Monitor application errors
   - Track suspicious activities

7. **Regular Updates**
   - Keep .NET runtime updated
   - Update NuGet packages regularly

---

## Recommended: Railway Quick Start

For the fastest deployment, I recommend **Railway**:

```bash
# 1. Already have code pushed to GitHub ✅

# 2. Go to railway.app and sign in with GitHub

# 3. New Project → Deploy from GitHub → Select your repo

# 4. Add PostgreSQL: New → Database → PostgreSQL

# 5. Done! Railway auto-configures everything
```

Your app will be live in ~5 minutes at: `https://your-app.railway.app`

---

## Cost Comparison

| Platform | Free Tier | Paid Plans | Database |
|----------|-----------|------------|----------|
| Railway | 500 hrs/month | $5/month | Included |
| Render | Yes (spins down) | $7/month | Free PostgreSQL |
| Azure | 60 min/day | $13/month | Separate cost |
| DigitalOcean | No | $5/month | $15/month |
| Heroku | No | $7/month | $5/month |

---

## Support

If you encounter issues:
1. Check platform-specific documentation
2. Review application logs
3. Verify environment variables
4. Test database connectivity
5. Check GitHub issues for similar problems

---

**Good luck with your deployment! 🚀**
