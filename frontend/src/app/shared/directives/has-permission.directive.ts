import { Directive, Input, TemplateRef, ViewContainerRef, OnInit, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AuthService } from '../../core/services/auth.service';

/**
 * Has Permission Directive
 * Shows/hides element based on user permissions
 * Usage: *appHasPermission="'patients:read'" or [appHasPermission]="['patients:read', 'patients:write']"
 */
@Directive({
  selector: '[appHasPermission]',
  standalone: true,
})
export class HasPermissionDirective implements OnInit, OnDestroy {
  private permissions: string[] = [];
  private requireAll = false;
  private destroy$ = new Subject<void>();

  @Input()
  set appHasPermission(permissions: string | string[]) {
    this.permissions = Array.isArray(permissions) ? permissions : [permissions];
    this.updateView();
  }

  @Input()
  set appHasPermissionRequireAll(requireAll: boolean) {
    this.requireAll = requireAll;
    this.updateView();
  }

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    // Re-check permissions when user changes
    this.authService.isAuthenticated$.pipe(takeUntil(this.destroy$)).subscribe(() => {
      this.updateView();
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private updateView(): void {
    if (this.hasPermission()) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainer.clear();
    }
  }

  private hasPermission(): boolean {
    if (this.permissions.length === 0) {
      return true;
    }

    if (this.requireAll) {
      return this.permissions.every((permission) =>
        this.checkPermission(permission)
      );
    } else {
      return this.permissions.some((permission) =>
        this.checkPermission(permission)
      );
    }
  }

  private checkPermission(permission: string): boolean {
    const [resource, action] = permission.split(':');
    return this.authService.hasPermission(resource, action);
  }
}
