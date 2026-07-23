# Deployment Guide

Complete deployment procedures for development, staging, and production environments.

---

## 🚀 Quick Deploy Summary

| Environment | Infrastructure | Target | Automation | RTO |
|-------------|----------------|--------|-----------|-----|
| **Local** | Docker Compose | Developer laptop | Manual | - |
| **Staging** | Azure AKS | Cloud (HA) | GitHub Actions | < 30 min |
| **Production** | Azure AKS | Cloud (HA, DR) | GitHub Actions + Approval | < 60 min |

---

## 💻 Local Development Deployment

### Prerequisites

```bash
# Install required tools
- Node.js 18+
- .NET 8 SDK
- Docker Desktop
- Git
- Visual Studio Code or Visual Studio 2022
```

### Setup Steps

```bash
# 1. Clone repository
git clone https://github.com/yourorg/modern-ehr-platform.git
cd modern-ehr-platform

# 2. Start local environment with Docker Compose
docker-compose up -d

# 3. Verify services are running
docker-compose ps

# Output should show:
# NAME                STATUS
# ehr-frontend        Up
# ehr-backend         Up
# ehr-sql-server      Up
```

### Accessing Services

```
Frontend:     http://localhost:4200
Backend API:  http://localhost:5000
Swagger UI:   http://localhost:5000/swagger
SQL Server:   localhost:1433

Credentials:
- DB User: sa
- DB Password: EhrPlatform@123! (from .env)
- Demo User: doctor@hospital.com / Password123!
```

### Troubleshooting

```bash
# View logs
docker-compose logs -f frontend
docker-compose logs -f backend
docker-compose logs -f sql-server

# Restart services
docker-compose restart frontend

# Clean rebuild
docker-compose down -v
docker-compose build --no-cache
docker-compose up -d

# Check port conflicts
netstat -ano | findstr :4200
```

---

## 🌥️ Azure Deployment

### Prerequisites

```bash
# Install Azure CLI
winget install microsoft.azurecli

# Install kubectl
az aks install-cli

# Install Helm
choco install kubernetes-helm

# Login to Azure
az login
az account set --subscription "<subscription-id>"
```

### Step 1: Create Azure Resources

```bash
# Set variables
$resourceGroup = "ehr-platform-prod"
$location = "eastus"
$clusterName = "ehr-aks-prod"
$acrName = "ehrplatformacr"
$sqlServerName = "ehr-sql-server"
$dbName = "ehr_platform_db"

# 1. Create resource group
az group create --name $resourceGroup --location $location

# 2. Create Azure Container Registry
az acr create --resource-group $resourceGroup \
  --name $acrName --sku Standard

# 3. Create AKS cluster
az aks create --resource-group $resourceGroup \
  --name $clusterName \
  --node-count 3 \
  --vm-set-type VirtualMachineScaleSets \
  --load-balancer-sku standard \
  --enable-managed-identity \
  --network-plugin azure \
  --docker-bridge-address 172.17.0.1/16 \
  --enable-cluster-autoscaling \
  --min-count 3 --max-count 10

# 4. Get AKS credentials
az aks get-credentials --resource-group $resourceGroup --name $clusterName

# 5. Create SQL Server
az sql server create --resource-group $resourceGroup \
  --name $sqlServerName \
  --location $location \
  --admin-user sqladmin \
  --admin-password P@ssw0rd123!ehr

# 6. Create SQL Database
az sql db create --resource-group $resourceGroup \
  --server $sqlServerName \
  --name $dbName \
  --service-objective S0
```

### Step 2: Build & Push Docker Images

```bash
# Navigate to project root
cd modern-ehr-platform

# Build frontend image
docker build -f devops/docker/Dockerfile.frontend \
  -t $acrName.azurecr.io/ehr-frontend:latest \
  -t $acrName.azurecr.io/ehr-frontend:v1.0.0 \
  ./frontend

# Build backend image
docker build -f devops/docker/Dockerfile.backend \
  -t $acrName.azurecr.io/ehr-backend:latest \
  -t $acrName.azurecr.io/ehr-backend:v1.0.0 \
  ./backend

# Login to ACR
az acr login --name $acrName

# Push images
docker push $acrName.azurecr.io/ehr-frontend:latest
docker push $acrName.azurecr.io/ehr-backend:latest

# Verify images
az acr repository list --name $acrName
```

### Step 3: Deploy to AKS

```bash
# 1. Create namespace
kubectl create namespace ehr-production

# 2. Create secrets
kubectl create secret docker-registry acr-secret \
  --docker-server=$acrName.azurecr.io \
  --docker-username=<acr-username> \
  --docker-password=<acr-password> \
  --docker-email=devops@moderneHR.com \
  -n ehr-production

# 3. Create database connection secret
kubectl create secret generic db-connection \
  --from-literal=connectionString="Server=tcp:$sqlServerName.database.windows.net,1433;Initial Catalog=$dbName;Persist Security Info=False;User ID=sqladmin;Password=P@ssw0rd123!ehr;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" \
  -n ehr-production

# 4. Create JWT secret
kubectl create secret generic jwt-secret \
  --from-literal=signingKey="$(openssl rand -base64 32)" \
  -n ehr-production

# 5. Deploy using Helm
cd devops/kubernetes
helm install ehr-platform ./ehr-platform-chart \
  --namespace ehr-production \
  --values values-prod.yaml \
  --set image.registry=$acrName.azurecr.io \
  --set image.frontend.tag=latest \
  --set image.backend.tag=latest

# 6. Verify deployment
kubectl get pods -n ehr-production
kubectl get svc -n ehr-production
kubectl get ingress -n ehr-production
```

### Step 4: Configure DNS & SSL

```bash
# Get Ingress IP
$ingressIP = kubectl get service -n ehr-production \
  | grep "ehr-platform-ingress" | awk '{print $4}'

# Update DNS records
# moderneHRplatform.com  A  $ingressIP
# api.moderneHRplatform.com  A  $ingressIP

# SSL certificate (Let's Encrypt with cert-manager)
kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.12.0/cert-manager.crds.yaml

helm install cert-manager jetstack/cert-manager \
  --namespace cert-manager --create-namespace \
  --version v1.12.0

# Update Ingress with TLS (automatic with cert-manager)
# Ingress will auto-renew certificates 30 days before expiry
```

### Step 5: Configure Monitoring & Logging

```bash
# Enable Container Insights
az aks enable-addons --addons monitoring \
  --name $clusterName \
  --resource-group $resourceGroup

# Create Application Insights
az monitor app-insights component create \
  --app ehr-platform-insights \
  --location $location \
  --resource-group $resourceGroup

# View logs
kubectl logs -f deployment/ehr-backend -n ehr-production

# Port-forward for debugging
kubectl port-forward svc/ehr-backend 5000:80 -n ehr-production
```

---

## 🔄 GitHub Actions CI/CD Pipeline

### Pipeline Structure

```
Code Push to Main
    │
    ├─→ [Frontend Jobs]
    │   ├─→ Build & Test
    │   ├─→ Security Scan
    │   └─→ Build Docker Image
    │
    ├─→ [Backend Jobs]
    │   ├─→ Build & Test
    │   ├─→ Security Scan
    │   └─→ Build Docker Image
    │
    ├─→ [Integration Tests]
    │   └─→ E2E Tests on Staging
    │
    ├─→ [Security & Quality]
    │   ├─→ SonarQube Quality Gate
    │   ├─→ Container Scan (Trivy)
    │   └─→ Dependency Check
    │
    └─→ [Deployment]
        ├─→ Deploy to Staging (automatic)
        ├─→ Smoke Tests
        └─→ Wait for Manual Approval
            └─→ Deploy to Production
```

### Frontend CI Workflow

```yaml
# .github/workflows/ci-frontend.yml
name: Frontend CI

on:
  push:
    branches: [main, develop]
    paths: [frontend/**, .github/workflows/ci-frontend.yml]

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup Node
        uses: actions/setup-node@v3
        with:
          node-version: 18
      
      - name: Install dependencies
        run: npm ci --prefix frontend
      
      - name: Lint
        run: npm run lint --prefix frontend
      
      - name: Build
        run: npm run build --prefix frontend
        env:
          ANGULAR_APP_VERSION: ${{ github.run_number }}
      
      - name: Test
        run: npm run test:ci --prefix frontend
      
      - name: Upload coverage
        uses: codecov/codecov-action@v3
        with:
          files: ./frontend/coverage/lcov.info
      
      - name: Security scan
        run: npm audit --prefix frontend || true
        
      - name: Build Docker image
        run: |
          docker build -f devops/docker/Dockerfile.frontend \
            -t ${{ secrets.REGISTRY_NAME }}.azurecr.io/ehr-frontend:${{ github.sha }} \
            -t ${{ secrets.REGISTRY_NAME }}.azurecr.io/ehr-frontend:latest \
            ./frontend
      
      - name: Push to ACR
        run: |
          az acr login --name ${{ secrets.REGISTRY_NAME }}
          docker push ${{ secrets.REGISTRY_NAME }}.azurecr.io/ehr-frontend:${{ github.sha }}
          docker push ${{ secrets.REGISTRY_NAME }}.azurecr.io/ehr-frontend:latest
        env:
          REGISTRY_NAME: ${{ secrets.REGISTRY_NAME }}
```

### Backend CI Workflow

```yaml
# .github/workflows/ci-backend.yml
name: Backend CI

on:
  push:
    branches: [main, develop]
    paths: [backend/**, .github/workflows/ci-backend.yml]

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    services:
      sqlserver:
        image: mcr.microsoft.com/mssql/server:latest
        env:
          SA_PASSWORD: TestPassword123!
          ACCEPT_EULA: Y
        options: >-
          --health-cmd="/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P TestPassword123! -Q 'SELECT 1' || exit 1"
          --health-interval=10s
          --health-timeout=5s
          --health-retries=5
    
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 8.x
      
      - name: Restore dependencies
        run: dotnet restore
        working-directory: ./backend
      
      - name: Build
        run: dotnet build --configuration Release --no-restore
        working-directory: ./backend
      
      - name: Test
        run: dotnet test --configuration Release --no-build --verbosity normal
        working-directory: ./backend
        env:
          ConnectionStrings__DefaultConnection: "Server=localhost;User Id=SA;Password=TestPassword123!;Database=ehr_test;"
      
      - name: SonarQube scan
        uses: sonarsource/sonarcloud-github-action@master
        with:
          args: >
            -Dsonar.projectKey=moderneHR_backend
            -Dsonar.sources=./backend/src
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
          SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
      
      - name: Build Docker image
        run: |
          docker build -f devops/docker/Dockerfile.backend \
            -t ${{ secrets.REGISTRY_NAME }}.azurecr.io/ehr-backend:${{ github.sha }} \
            -t ${{ secrets.REGISTRY_NAME }}.azurecr.io/ehr-backend:latest \
            ./backend
      
      - name: Scan image for vulnerabilities
        uses: aquasecurity/trivy-action@master
        with:
          image-ref: ${{ secrets.REGISTRY_NAME }}.azurecr.io/ehr-backend:${{ github.sha }}
          format: 'sarif'
          output: 'trivy-results.sarif'
      
      - name: Upload Trivy results
        uses: github/codeql-action/upload-sarif@v2
        with:
          sarif_file: 'trivy-results.sarif'
      
      - name: Push to ACR
        run: |
          az acr login --name ${{ secrets.REGISTRY_NAME }}
          docker push ${{ secrets.REGISTRY_NAME }}.azurecr.io/ehr-backend:${{ github.sha }}
          docker push ${{ secrets.REGISTRY_NAME }}.azurecr.io/ehr-backend:latest
```

### Deployment Workflow

```yaml
# .github/workflows/deploy-prod.yml
name: Deploy to Production

on:
  workflow_run:
    workflows: [Frontend CI, Backend CI]
    types: [completed]
    branches: [main]

jobs:
  deploy-staging:
    if: ${{ github.event.workflow_run.conclusion == 'success' }}
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Connect to AKS
        run: |
          az login --service-principal \
            -u ${{ secrets.AZURE_CLIENT_ID }} \
            -p ${{ secrets.AZURE_CLIENT_SECRET }} \
            --tenant ${{ secrets.AZURE_TENANT_ID }}
          az aks get-credentials --resource-group ${{ secrets.AKS_RESOURCE_GROUP }} \
            --name ${{ secrets.AKS_CLUSTER_NAME }} \
            --overwrite-existing
      
      - name: Deploy to Staging
        run: |
          helm upgrade --install ehr-platform ./devops/kubernetes/ehr-platform-chart \
            --namespace staging \
            --values devops/kubernetes/values-staging.yaml \
            --set image.frontend.tag=${{ github.sha }} \
            --set image.backend.tag=${{ github.sha }} \
            --wait

  smoke-tests:
    needs: deploy-staging
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Run smoke tests
        run: |
          npm install --prefix frontend
          npm run e2e:smoke --prefix frontend
        env:
          API_BASE_URL: https://staging-api.moderneHRplatform.com

  approve-prod-deployment:
    needs: smoke-tests
    runs-on: ubuntu-latest
    environment: production
    steps:
      - name: Production deployment approved
        run: echo "Manual approval received"

  deploy-production:
    needs: approve-prod-deployment
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Connect to AKS
        run: |
          az login --service-principal \
            -u ${{ secrets.AZURE_CLIENT_ID }} \
            -p ${{ secrets.AZURE_CLIENT_SECRET }} \
            --tenant ${{ secrets.AZURE_TENANT_ID }}
          az aks get-credentials --resource-group ${{ secrets.AKS_RESOURCE_GROUP }} \
            --name ${{ secrets.AKS_CLUSTER_NAME }} \
            --overwrite-existing
      
      - name: Deploy to Production
        run: |
          helm upgrade --install ehr-platform ./devops/kubernetes/ehr-platform-chart \
            --namespace production \
            --values devops/kubernetes/values-prod.yaml \
            --set image.frontend.tag=${{ github.sha }} \
            --set image.backend.tag=${{ github.sha }} \
            --wait \
            --timeout 10m
      
      - name: Health checks
        run: |
          kubectl rollout status deployment/ehr-frontend -n production --timeout=5m
          kubectl rollout status deployment/ehr-backend -n production --timeout=5m
      
      - name: Notify deployment
        run: |
          # Send notification to Slack/Teams
          echo "Production deployment completed: ${{ github.sha }}"
```

---

## 🔍 Health Checks & Monitoring

### Health Endpoints

```bash
# Frontend
GET http://localhost:4200/health
Response: { status: "healthy" }

# Backend
GET http://localhost:5000/health
Response: { 
  status: "healthy",
  database: "connected",
  version: "1.0.0",
  timestamp: "2024-07-20T14:50:00Z"
}

# Database
GET http://localhost:5000/health/database
Response: { status: "connected", latency: "12ms" }
```

### Kubernetes Probes

```yaml
# From Kubernetes deployment manifest
livenessProbe:
  httpGet:
    path: /health
    port: 5000
  initialDelaySeconds: 30
  periodSeconds: 10
  failureThreshold: 3

readinessProbe:
  httpGet:
    path: /health/ready
    port: 5000
  initialDelaySeconds: 10
  periodSeconds: 5
  failureThreshold: 1
```

---

## 📊 Scaling & Performance

### Auto-scaling Configuration

```yaml
# Horizontal Pod Autoscaler
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: ehr-backend-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: ehr-backend
  minReplicas: 3
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80
```

### Performance Benchmarks

| Metric | Target | Measurement |
|--------|--------|-------------|
| API Response (p95) | < 500ms | Application Insights |
| Frontend Load Time | < 3s | Lighthouse |
| Database Query (p95) | < 200ms | SQL Profiler |
| Container Startup | < 30s | K8s event logs |
| Pod Scaling Time | < 2 min | Auto-scaler metrics |

---

## 🔄 Rollback Procedures

### Automatic Rollback

```bash
# If health check fails, K8s automatically:
1. Stops rolling updates
2. Keeps previous version running
3. Alerts operations team

# View rollback history
kubectl rollout history deployment/ehr-backend -n production

# Manual rollback
kubectl rollout undo deployment/ehr-backend -n production

# Rollback to specific revision
kubectl rollout undo deployment/ehr-backend --to-revision=5 -n production
```

### Database Rollback

```bash
# Keep previous database backups for 35 days
# In case of migration failure:

1. Identify failure time
2. Restore from backup
3. Replay transaction logs to specific point-in-time
4. Validate data integrity
5. Restart application
6. Monitor for issues
```

---

## 📞 Support & Troubleshooting

**Common Issues**:
- Pod crash loop: Check logs with `kubectl logs <pod> -n production`
- Image pull failed: Verify ACR credentials
- Database connection: Check network policies & firewall rules
- High latency: Check HPA status, consider scaling

**Escalation**: On-call DevOps engineer

---

**Version**: 1.0.0 | Last Updated: July 2026
