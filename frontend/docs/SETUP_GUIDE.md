# Frontend Setup Guide

Complete setup instructions for Modern EHR Platform Angular frontend development.

---

## 📋 Prerequisites

### System Requirements

- **Node.js**: 18.0.0 or higher
- **npm**: 9.0.0 or higher
- **Git**: 2.30+
- **Editor**: VS Code (recommended) or equivalent
- **Disk Space**: 3GB minimum

### Verify Installation

```bash
node --version    # Should be v18.x.x or higher
npm --version     # Should be 9.x.x or higher
git --version     # Should be 2.30+
```

---

## 🚀 Quick Start (5 minutes)

```bash
# 1. Navigate to frontend directory
cd frontend

# 2. Install dependencies
npm install

# 3. Start development server
npm start

# 4. Open browser
# http://localhost:4200

# 5. Login with demo credentials
# Email: doctor@hospital.com
# Password: Password123!
```

---

## 🛠️ Detailed Setup

### Step 1: Install Dependencies

```bash
cd frontend

# Clean install (recommended first time)
rm -rf node_modules package-lock.json
npm install

# Or quick install if node_modules exists
npm install
```

**Troubleshooting**:
```bash
# If npm install fails:
npm cache clean --force
npm install

# If specific package fails:
npm install <package-name>

# For permission issues (macOS/Linux):
sudo npm install
```

### Step 2: Configure Environment

```bash
# Copy environment template
cp src/environments/environment.example.ts src/environments/environment.ts

# Edit environment.ts if needed
# Default local configuration:
# - API Base URL: http://localhost:5000
# - Auth URL: http://localhost:5000/auth
```

**environment.ts Structure**:
```typescript
export const environment = {
  production: false,
  apiBaseUrl: 'http://localhost:5000/api/v1',
  authUrl: 'http://localhost:5000/auth',
  logLevel: 'debug',
  mockData: true,  // Use mock data instead of real API
  features: {
    enableMFA: false,
    enableDarkMode: true,
    enableI18n: true
  }
};
```

### Step 3: Start Development Server

```bash
# Development mode with hot reload
npm start

# Or specify port
ng serve --port 4300

# Enable proxy for API calls (if needed)
ng serve --proxy-config proxy.conf.json
```

**Expected Output**:
```
✔ Compiled successfully.
✔ Build cache populated from 15 sources.

Now serving Angular app on: http://localhost:4200 - Local: http://localhost:4200/ - external: http://192.168.1.x:4200/
```

### Step 4: Verify Setup

```bash
# Open browser and navigate to
http://localhost:4200

# You should see:
✅ Modern EHR Platform login page
✅ Input fields for email/password
✅ Demo credentials link (optional)

# Login with:
Email: doctor@hospital.com
Password: Password123!

# You should see:
✅ Dashboard with patient list
✅ Sidebar navigation
✅ Top navigation bar
```

---

## 📦 Build Commands

### Development Build

```bash
npm run dev
# or
ng serve

# Options:
ng serve --port 4300                    # Custom port
ng serve --poll 2000                    # File polling interval
ng serve --disable-host-check           # Disable host validation
```

### Production Build

```bash
npm run build

# Or with source maps (for debugging)
npm run build:prod
npm run build:prod:sourcemaps

# Or with profiling
ng build --stats-json
```

**Build Output**:
```
dist/modern-ehr-frontend/
├── index.html
├── main.*.js         (Main application bundle)
├── polyfills.*.js    (Browser compatibility)
├── styles.*.css      (Tailwind CSS)
└── assets/           (Images, icons, etc.)
```

### Build Analysis

```bash
# Check bundle size
npm run analyze

# Expected sizes (gzipped):
# main.*.js: < 150KB
# Total: < 400KB

# Visualize bundle
npm run bundle-report
```

---

## 🧪 Testing

### Unit Tests

```bash
# Run tests once
npm test

# Run tests in watch mode (recommended for development)
npm run test:watch

# Run tests with coverage
npm run test:coverage

# Expected coverage: > 80%
```

**Test File Locations**:
```
src/
├── app/shared/components/button.component.spec.ts
├── app/core/services/auth.service.spec.ts
├── app/features/patients/services/patient.service.spec.ts
└── ... (one .spec.ts file per .ts file)
```

### E2E Tests

```bash
# Run Cypress E2E tests
npm run e2e

# Run headless (CI mode)
npm run e2e:headless

# Open Cypress Test Runner (interactive)
npm run e2e:open

# Run specific test file
npm run e2e:run -- --spec "cypress/e2e/auth.e2e.cy.ts"
```

**Test Coverage Areas**:
- ✅ Authentication (login, logout, token refresh)
- ✅ Patient Management (search, create, update)
- ✅ Appointments (schedule, cancel, reschedule)
- ✅ Prescriptions (create, refill)
- ✅ Medical Records (view, create SOAP notes)
- ✅ Error Handling (network failures, validation)

### Debugging Tests

```bash
# Run single test with debugging
ng test --browsers=Chrome --watch=true

# In Chrome DevTools:
# 1. Open Sources tab
# 2. Find test file
# 3. Set breakpoints
# 4. Run test

# Or use Visual Studio Code debugger
# See .vscode/launch.json for configuration
```

---

## 🎨 Code Quality

### Linting

```bash
# Run ESLint
npm run lint

# Fix auto-fixable issues
npm run lint:fix

# Run prettier (code formatting)
npm run format

# Check formatting without fixing
npm run format:check
```

**Configuration Files**:
- `.eslintrc.json` - Linting rules
- `.prettierrc` - Code formatting
- `tsconfig.json` - TypeScript settings

### Code Formatting

```bash
# Format all TypeScript files
npm run format

# Format specific file
npx prettier --write src/app/app.component.ts

# Check formatting
npm run format:check
```

---

## 🔍 Development Tools

### VS Code Extensions (Recommended)

```
Extensions to install:
- Angular Language Service (Angular)
- Angular Snippets (johnpapa.Angular2)
- Prettier - Code formatter (esbenp.prettier-vscode)
- ESLint (dbaeumer.vscode-eslint)
- Thunder Client (rangav.vscode-thunder-client) for API testing
- GitLens (eamodio.gitlens)
- Material Icon Theme (PKief.material-icon-theme)
- Tailwind CSS IntelliSense (bradlc.vscode-tailwindcss)
```

### Browser DevTools

```
Chrome DevTools Features:
- Network Tab: Monitor API calls
- Application Tab: View local storage, cookies, service workers
- Performance Tab: Profile app performance
- Console: View errors and logs

Angular DevTools Extension:
- Install: chrome.google.com/webstore
- View component tree
- Inspect component properties
- Monitor change detection
```

### API Testing

```bash
# Use Thunder Client (VS Code) or Postman
# Base URL: http://localhost:5000/api/v1

# Example requests:
GET /patients
GET /patients/123
POST /appointments (with body)
PUT /patients/123
DELETE /patients/123

# Bearer token format:
Authorization: Bearer <jwt_token_here>
```

---

## 🌍 Environment Variables

### Local Development (.env)

```bash
# Create .env file in frontend root
NG_APP_API_BASE_URL=http://localhost:5000/api/v1
NG_APP_AUTH_URL=http://localhost:5000/auth
NG_APP_LOG_LEVEL=debug
NG_APP_USE_MOCK_DATA=true
NG_APP_FEATURE_MFA=false
NG_APP_FEATURE_DARK_MODE=true
NG_APP_FEATURE_I18N=true
```

### Staging (.env.staging)

```bash
NG_APP_API_BASE_URL=https://staging-api.moderneHRplatform.com/api/v1
NG_APP_AUTH_URL=https://staging-api.moderneHRplatform.com/auth
NG_APP_LOG_LEVEL=info
NG_APP_USE_MOCK_DATA=false
NG_APP_FEATURE_MFA=true
NG_APP_FEATURE_DARK_MODE=true
NG_APP_FEATURE_I18N=true
```

### Production (.env.prod)

```bash
NG_APP_API_BASE_URL=https://api.moderneHRplatform.com/api/v1
NG_APP_AUTH_URL=https://api.moderneHRplatform.com/auth
NG_APP_LOG_LEVEL=error
NG_APP_USE_MOCK_DATA=false
NG_APP_FEATURE_MFA=true
NG_APP_FEATURE_DARK_MODE=true
NG_APP_FEATURE_I18N=true
```

---

## 🐛 Common Issues & Fixes

### Port Already in Use

```bash
# Kill process on port 4200
# Windows
netstat -ano | findstr :4200
taskkill /PID <PID> /F

# macOS/Linux
lsof -i :4200
kill -9 <PID>

# Or use different port
ng serve --port 4300
```

### Module Not Found

```bash
# Clear node_modules and reinstall
rm -rf node_modules package-lock.json
npm install

# Or install missing module
npm install <module-name>
```

### TypeScript Errors

```bash
# Rebuild TypeScript
npm run build

# Check tsconfig.json for strict mode settings
# If too strict, temporarily disable:
"strict": false  // Only for development!
```

### CORS Issues

```bash
# Use proxy.conf.json to forward API requests
ng serve --proxy-config proxy.conf.json

# Or configure backend CORS headers
# Backend should return:
# Access-Control-Allow-Origin: http://localhost:4200
# Access-Control-Allow-Credentials: true
```

### Hot Module Replacement (HMR) Not Working

```bash
# Clear browser cache
# Ctrl+Shift+Delete (Windows/Linux)
# Cmd+Shift+Delete (macOS)

# Or restart dev server
npm start
```

---

## 📚 Useful NPM Scripts

| Script | Purpose |
|--------|---------|
| `npm start` | Start dev server |
| `npm test` | Run unit tests |
| `npm run test:watch` | Tests in watch mode |
| `npm run test:coverage` | Coverage report |
| `npm run e2e` | E2E tests |
| `npm run e2e:headless` | E2E tests headless |
| `npm run lint` | Run ESLint |
| `npm run lint:fix` | Fix linting issues |
| `npm run format` | Format code with Prettier |
| `npm run build` | Production build |
| `npm run build:prod` | Optimized build |
| `npm run analyze` | Bundle analysis |
| `npm run serve:prod` | Serve production build locally |

---

## 🔐 Security in Development

### Secure Credentials

```bash
# Never commit secrets to Git
# Use .env files (add to .gitignore)

.gitignore:
.env
.env.local
.env.*.local
src/environments/environment.local.ts
```

### API Key Management

```typescript
// ❌ DON'T: Hardcode API keys
const API_KEY = 'sk_live_abc123';

// ✅ DO: Use environment variables
const API_KEY = environment.apiKey;  // From .env
```

### Local Storage Security

```typescript
// ❌ DON'T: Store sensitive data in localStorage
localStorage.setItem('password', password);

// ✅ DO: Use HTTP-only cookies or secure storage
// Backend sets: Set-Cookie: token=...; HttpOnly; Secure
```

---

## 🚀 Deployment Checklist

Before deploying:

- [ ] All tests pass: `npm test` & `npm run e2e`
- [ ] No linting errors: `npm run lint`
- [ ] Build succeeds: `npm run build`
- [ ] No console errors in browser
- [ ] Environment variables configured
- [ ] API endpoints verified
- [ ] Security headers checked
- [ ] Performance acceptable (< 3s load time)
- [ ] Accessibility audit passed
- [ ] Cross-browser tested

---

## 📞 Getting Help

- **Angular Docs**: https://angular.io
- **Tailwind CSS**: https://tailwindcss.com
- **TypeScript**: https://www.typescriptlang.org
- **RxJS**: https://rxjs.dev
- **Issue Tracker**: GitHub Issues

---

**Version**: 1.0.0 | Last Updated: July 2026
