# Modern EHR Platform - Frontend

Enterprise-grade Electronic Health Records (EHR) system frontend built with Angular 18+, Tailwind CSS, and NgRx.

## Features

- **Scalable Architecture**: Feature-based modular design with lazy loading
- **Authentication & Authorization**: JWT-based auth with role-based access control (RBAC)
- **State Management**: NgRx for complex global state + Signals for local state
- **Responsive Design**: Mobile-first approach with Tailwind CSS
- **Internationalization**: i18n support for multiple languages (EN, AR) with RTL
- **Accessibility**: WCAG AA compliance
- **Dark Mode**: Full dark mode support
- **Real-time Updates**: WebSocket integration ready
- **HIPAA-Ready**: Security patterns and audit logging

## Project Structure

```
src/app/
+-- core/               # App-wide singletons (auth, services, models)
+-- shared/             # Reusable components, pipes, directives
+-- features/           # Business domains (lazy-loaded)
¦   +-- auth/          # Authentication
¦   +-- dashboard/     # Dashboard
¦   +-- patients/      # Patient management
¦   +-- appointments/  # Appointment scheduling
¦   +-- medical-records/
¦   +-- prescriptions/
¦   +-- lab-results/
¦   +-- billing/
¦   +-- reports-analytics/
¦   +-- admin/         # Administration & RBAC
+-- layouts/           # Page layouts
+-- routes/            # Routing configuration
+-- store/             # NgRx root state
```

## Installation

### Prerequisites

- Node.js 18+ and npm 9+
- Angular 18+
- TypeScript 5.3+

### Setup

```bash
# Install dependencies
npm install

# Start development server
npm start

# Open browser
http://localhost:4200
```

## Development

```bash
# Build
npm run build

# Build for production
npm run build:prod

# Run tests
npm run test

# Run tests with coverage
npm run test:coverage

# Run linter
npm run lint

# Run e2e tests
npm run e2e
```

## Configuration

### Environment Variables

Create `.env` file in the root:

```env
API_URL=http://localhost:3000/api
WS_URL=ws://localhost:3000
```

### Tailwind CSS

Tailwind is pre-configured. Customize theme in `tailwind.config.js`.

### i18n

Translation files are in `src/assets/i18n/`:
- `en.json` - English
- `ar.json` - Arabic (RTL)

## Architecture Patterns

### Component Design

- **Standalone Components**: All components are standalone (Angular 14+)
- **OnPush Change Detection**: Used by default for performance
- **Smart/Dumb Components**: Separation of concerns

### State Management

```typescript
// Global state with NgRx
@NgModule({
  imports: [StoreModule.forRoot(appReducers)]
})

// Local state with Signals
private count = signal(0);
readonly doubleCount = computed(() => this.count() * 2);
```

### Routing

- **Lazy Loading**: All feature modules are lazy-loaded
- **Preloading Strategy**: PreloadAllModules for optimal UX
- **Guards**: Auth and Role-based route guards

## Security

- JWT token management with refresh token rotation
- CORS-enabled API calls
- XSS protection through Angular's built-in sanitization
- CSRF token handling in interceptors
- Audit logging for compliance

## Performance

- Tree-shaking optimized
- Code splitting with lazy loading
- OnPush change detection strategy
- Preloading strategies configured
- Bundle size < 500KB (gzipped)

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Contributing

1. Create a feature branch
2. Follow Angular style guide
3. Write tests for new features
4. Submit PR with description

## License

Proprietary - All rights reserved

## Support

For issues and questions, please contact the development team.
