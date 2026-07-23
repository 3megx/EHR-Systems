import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth.service';
import { MOCK_USERS } from '@shared/mock-data';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService],
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);

    // Clear localStorage
    localStorage.clear();
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should login successfully', (done) => {
    const credentials = { email: 'doctor@ehr.com', password: 'password123' };
    const mockUser = MOCK_USERS[0];
    const mockResponse = {
      user: mockUser,
      token: {
        accessToken: 'test-token',
        refreshToken: 'refresh-token',
        expiresIn: 3600,
        tokenType: 'Bearer',
      },
    };

    service.login(credentials).subscribe({
      next: (response) => {
        expect(response.user).toEqual(mockUser);
        expect(service.getToken()).toBe('test-token');
        done();
      },
      error: () => fail('login should have succeeded'),
    });

    const req = httpMock.expectOne((request) => request.url.includes('/auth/login'));
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  it('should check if user is authenticated', (done) => {
    const mockResponse = {
      user: MOCK_USERS[0],
      token: {
        accessToken: 'test-token',
        refreshToken: 'refresh-token',
        expiresIn: 3600,
        tokenType: 'Bearer',
      },
    };

    service.login({ email: 'doctor@ehr.com', password: 'password' }).subscribe(() => {
      expect(service.isAuthenticated()).toBe(true);
      done();
    });

    httpMock.expectOne((request) => request.url.includes('/auth/login')).flush(mockResponse);
  });

  it('should check user roles', (done) => {
    const mockResponse = {
      user: MOCK_USERS[0], // Doctor role
      token: {
        accessToken: 'test-token',
        refreshToken: 'refresh-token',
        expiresIn: 3600,
        tokenType: 'Bearer',
      },
    };

    service.login({ email: 'doctor@ehr.com', password: 'password' }).subscribe(() => {
      expect(service.hasRole('doctor')).toBe(true);
      expect(service.hasRole('nurse')).toBe(false);
      done();
    });

    httpMock.expectOne((request) => request.url.includes('/auth/login')).flush(mockResponse);
  });

  it('should logout successfully', (done) => {
    service.logout().subscribe(() => {
      expect(service.isAuthenticated()).toBe(false);
      expect(service.getToken()).toBeNull();
      done();
    });

    httpMock.expectOne((request) => request.url.includes('/auth/logout')).flush({});
  });
});
