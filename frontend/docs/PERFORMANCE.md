# Performance Optimization Guide

Best practices and techniques for optimizing Modern EHR Platform frontend performance.

---

## 🎯 Performance Targets

| Metric | Target | Tool |
|--------|--------|------|
| **Lighthouse Score** | > 90 | Lighthouse |
| **First Contentful Paint** | < 1.5s | Web Vitals |
| **Largest Contentful Paint** | < 2.5s | Web Vitals |
| **Cumulative Layout Shift** | < 0.1 | Web Vitals |
| **Time to Interactive** | < 3s | Lighthouse |
| **Bundle Size** | < 400KB (gzipped) | webpack-bundle-analyzer |
| **Main Thread Time** | < 3.8s | Lighthouse |

---

## 📦 Bundle Size Optimization

### Analyze Bundle

```bash
# Generate bundle report
npm run analyze

# Visualize with webpack-bundle-analyzer
npm run build
npm run bundle-report

# Expected output:
# main.*.js: 120-150KB (gzipped)
# Total: 300-400KB (gzipped)
```

### Reduce Bundle Size

#### 1. Remove Unused Dependencies

```bash
# Find unused packages
npm ls
npm prune

# Remove unused packages
npm uninstall unused-package
```

#### 2. Code Splitting & Lazy Loading

```typescript
// app.routes.ts - Already implemented with lazy loading
export const routes: Routes = [
  {
    path: 'patients',
    loadComponent: () => import('./features/patients/pages/patient-list-page/patient-list-page.component')
      .then(m => m.PatientListPageComponent)
  }
];
```

#### 3. Tree Shaking

```typescript
// ✅ Good: Named imports enable tree-shaking
import { Component, OnInit } from '@angular/core';

// ❌ Bad: Prevents tree-shaking
import * as ng from '@angular/core';
```

#### 4. Build Optimization

```bash
# Production build with optimization
npm run build:prod

# Angular performs:
# ✅ Minification
# ✅ Tree-shaking
# ✅ Ahead-of-Time (AOT) compilation
# ✅ Differential loading (ES2015 + ES5)
```

---

## ⚡ Runtime Performance

### Change Detection Strategy

```typescript
// ✅ Use OnPush for better performance
@Component({
  selector: 'app-patient-card',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<div>{{ patient.name }}</div>`,
  standalone: true
})
export class PatientCardComponent {
  @Input() patient: Patient;
}

// ❌ Default strategy checks entire tree on every change
```

### Memoization & Caching

```typescript
// Cache HTTP responses
@Injectable({ providedIn: 'root' })
export class PatientService {
  private cache = new Map<string, Observable<Patient[]>>();
  
  searchPatients(query: string): Observable<Patient[]> {
    if (this.cache.has(query)) {
      return this.cache.get(query)!;
    }
    
    const result$ = this.http.get<Patient[]>(`/api/patients?q=${query}`)
      .pipe(shareReplay(1));
    
    this.cache.set(query, result$);
    return result$;
  }
}
```

### Virtual Scrolling (for large lists)

```typescript
import { ScrollingModule } from '@angular/cdk/scrolling';

@Component({
  selector: 'app-large-patient-list',
  template: `
    <cdk-virtual-scroll-viewport itemSize="50" class="patients-list">
      <div *cdkVirtualFor="let patient of patients">
        <app-patient-row [patient]="patient"></app-patient-row>
      </div>
    </cdk-virtual-scroll-viewport>
  `,
  imports: [ScrollingModule, PatientRowComponent]
})
export class LargePatientListComponent {
  patients: Patient[] = [];  // Large list (10,000+)
}
```

### Debouncing & Throttling

```typescript
// Debounce search input
@Component({
  selector: 'app-patient-search',
  template: `
    <input [formControl]="searchControl" placeholder="Search..."/>
    <div *ngIf="results$ | async as results">
      <div *ngFor="let patient of results">{{ patient.name }}</div>
    </div>
  `,
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule]
})
export class PatientSearchComponent {
  searchControl = new FormControl('');
  
  results$ = this.searchControl.valueChanges.pipe(
    debounceTime(300),  // Wait 300ms after user stops typing
    distinctUntilChanged(),
    switchMap(query => 
      query ? this.patientService.search(query) : of([])
    )
  );
  
  constructor(private patientService: PatientService) {}
}
```

---

## 🖼️ Image Optimization

### Use Angular Image Directive

```typescript
// ✅ GOOD: Use Angular directive for automatic optimization
import { NgOptimizedImage } from '@angular/common';

@Component({
  template: `
    <img ngSrc="path/to/image.png" 
         width="200" 
         height="150" 
         alt="Patient photo"/>
  `,
  imports: [NgOptimizedImage],
  standalone: true
})
export class PatientPhotoComponent {}

// ❌ AVOID: Plain img tag
<img src="path/to/image.png" alt="Patient photo"/>
```

### Image Format & Size

```bash
# Convert to WebP (better compression)
# Use AVIF for even better compression
# Provide fallback formats

<!-- Picture element for format fallback -->
<picture>
  <source srcset="image.avif" type="image/avif">
  <source srcset="image.webp" type="image/webp">
  <img src="image.png" alt="Patient photo">
</picture>
```

### Responsive Images

```html
<!-- Serve different sizes for different viewports -->
<img 
  srcset="
    /images/patient-small.webp 480w,
    /images/patient-medium.webp 800w,
    /images/patient-large.webp 1200w"
  sizes="
    (max-width: 480px) 100vw,
    (max-width: 800px) 50vw,
    33vw"
  src="/images/patient-large.webp"
  alt="Patient photo">
```

---

## 🎨 CSS Optimization

### CSS-in-JS vs External CSS

```typescript
// ✅ Use external Tailwind CSS (better for treeshaking)
@Component({
  selector: 'app-button',
  template: `<button class="px-4 py-2 bg-blue-500">Click</button>`,
  standalone: true
})
export class ButtonComponent {}

// ❌ Inline styles are not optimized
@Component({
  template: `<button [style]="buttonStyle">Click</button>`,
  styles: [`button { padding: 8px 16px; }`]
})
export class BadButtonComponent {
  buttonStyle = { padding: '8px', backgroundColor: 'blue' };
}
```

### CSS Class Generation

```typescript
// ✅ Use static classes
<div class="text-lg font-bold text-blue-600">Patient Name</div>

// ❌ Avoid dynamic class generation (not tree-shakeable)
<div [class]="'text-' + size + ' font-' + weight">Patient Name</div>
```

---

## 🚀 Network Optimization

### HTTP Caching

```typescript
// Set proper cache headers in HTTP interceptor
@Injectable()
export class CacheInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpResponse<any>> {
    // Cache GET requests for 1 hour
    if (req.method === 'GET') {
      return next.handle(req).pipe(
        tap(response => {
          if (response instanceof HttpResponse) {
            response.headers.set('Cache-Control', 'max-age=3600');
          }
        })
      );
    }
    return next.handle(req);
  }
}
```

### Request Batching

```typescript
// Batch multiple requests
const patients$ = forkJoin([
  this.patientService.getPatient(1),
  this.patientService.getPatient(2),
  this.patientService.getPatient(3)
]);
```

### Compression

```bash
# Backend should send gzip-compressed responses
# Verify in browser:
# Network tab → Response headers → Content-Encoding: gzip
```

### Content Delivery Network (CDN)

```bash
# Deploy static assets to CDN
# In production build:
# - Images: CDN URL
# - Static assets: CDN URL
# - API calls: Direct to backend
```

---

## 📱 Mobile Optimization

### Responsive Web Design

```typescript
// Already implemented with Tailwind CSS
// sm: (640px), md: (768px), lg: (1024px), xl: (1280px), 2xl: (1536px)

@Component({
  template: `
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3">
      <!-- Responsive grid -->
    </div>
  `
})
export class ResponsiveComponent {}
```

### Mobile-First CSS

```typescript
// Write mobile styles first, then add media queries
@Component({
  template: `
    <div class="p-2 md:p-4 lg:p-6">
      <!-- Padding: 8px on mobile, 16px on tablet, 24px on desktop -->
    </div>
  `
})
export class MobileFirstComponent {}
```

---

## 🔍 Performance Monitoring

### Web Vitals Monitoring

```typescript
// Measure Core Web Vitals
import { getCLS, getFID, getFCP, getLCP, getTTFB } from 'web-vitals';

getCLS(console.log);  // Cumulative Layout Shift
getFID(console.log);  // First Input Delay
getFCP(console.log);  // First Contentful Paint
getLCP(console.log);  // Largest Contentful Paint
getTTFB(console.log); // Time to First Byte
```

### Custom Performance Marks

```typescript
// Measure specific operations
performance.mark('patient-search-start');
// ... search operation ...
performance.mark('patient-search-end');
performance.measure('patient-search', 'patient-search-start', 'patient-search-end');

const measure = performance.getEntriesByName('patient-search')[0];
console.log(`Search took ${measure.duration}ms`);
```

### Lighthouse Audits

```bash
# Run Lighthouse in Chrome
# DevTools → Lighthouse → Generate report

# Or CLI
npm install -g lighthouse
lighthouse https://moderneHRplatform.com --view
```

---

## 🛠️ Profiling & Debugging

### Chrome DevTools Performance Tab

```
1. Open DevTools (F12)
2. Go to Performance tab
3. Click record (or Ctrl+Shift+E)
4. Interact with app
5. Stop recording
6. Analyze flame chart
```

### Angular DevTools

```
1. Install Angular DevTools Chrome extension
2. Open DevTools → Angular tab
3. View component tree
4. Check change detection triggers
5. Monitor performance metrics
```

### Memory Profiling

```
1. DevTools → Memory tab
2. Take heap snapshot
3. Interact with app
4. Take another snapshot
5. Compare for memory leaks
```

---

## 📊 Performance Budget

### JavaScript Budget

```
main.*.js:           150KB (gzipped)
vendor.*.js:         100KB (gzipped)
polyfills.*.js:       50KB (gzipped)
────────────────────────────
Total:               300KB (gzipped)
```

### CSS Budget

```
styles.*.css:        100KB (gzipped)
```

### Total Budget: 400KB (gzipped)

### Monitor Budget

```bash
# Fail build if bundle exceeds budget
ng build --configuration=production --stats-json

# Analyze with webpack-bundle-analyzer
npm run bundle-report
```

---

## ✅ Performance Checklist

### Before Deployment

- [ ] Lighthouse score > 90
- [ ] Bundle size < 400KB (gzipped)
- [ ] No unused dependencies
- [ ] Lazy loading enabled for features
- [ ] Images optimized (WebP format)
- [ ] CSS tree-shaking enabled
- [ ] HTTP caching configured
- [ ] Compression enabled (gzip)
- [ ] CDN configured for static assets
- [ ] Service worker enabled (PWA)
- [ ] No console errors/warnings
- [ ] Mobile performance tested
- [ ] Web Vitals within targets
- [ ] Performance budget adhered to

---

## 📈 Improvement Timeline

### Phase 1: Quick Wins (Week 1)
- Lazy loading implementation
- Remove unused packages
- Enable production build optimizations

### Phase 2: Medium Effort (Week 2-3)
- Image optimization
- HTTP caching
- Code splitting

### Phase 3: Long Term (Month 2+)
- Service worker optimization
- Advanced caching strategies
- CDN integration
- Performance monitoring

---

## 📚 Resources

- [Google Lighthouse](https://developers.google.com/web/tools/lighthouse)
- [Web Vitals](https://web.dev/vitals/)
- [Angular Performance Guide](https://angular.io/guide/performance-best-practices)
- [Chrome DevTools](https://developer.chrome.com/docs/devtools/)
- [Webpack Optimization](https://webpack.js.org/guides/production/)

---

**Version**: 1.0.0 | Last Updated: July 2026
