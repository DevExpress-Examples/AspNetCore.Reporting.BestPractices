import { of } from 'rxjs';

export function createActivatedRouteStub(actionPath: string) {
  return {
    snapshot: {
      url: [{ path: 'authentication' }, { path: actionPath }],
      queryParamMap: {
        get: () => null
      },
      queryParams: {}
    }
  };
}

export function createAuthorizeServiceStub() {
  return {
    isAuthenticated: () => of(false),
    getUser: () => of(null),
    signIn: async () => ({ status: 1, state: null }),
    completeSignIn: async () => ({ status: 1, state: null }),
    signOut: async () => ({ status: 1, state: null }),
    completeSignOut: async () => ({ status: 1, state: null })
  };
}
