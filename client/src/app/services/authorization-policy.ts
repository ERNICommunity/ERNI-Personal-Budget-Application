import { UserInfo } from '../model/userInfo';

export class AuthorizationPolicy {
    public static evaluate(
        policyName: PolicyNames,
        userInfo: UserInfo | undefined
    ) {
        if (!userInfo) {
            return false;
        }

        return AuthorizationPolicy[policyName](userInfo);
    }

    public static isAdmin(user: UserInfo | null | undefined): boolean {
        if (!user) {
            return false;
        }

        return user.isAdmin;
    }

    public static canReadRequests(user: UserInfo | null | undefined): boolean {
        if (!user) {
            return false;
        }

        return user.isAdmin || user.isFinance;
    }

    public static canAccessMyBudget(
        user: UserInfo | null | undefined
    ): boolean {
        if (!user) {
            return false;
        }

        return true;
    }

    public static isSuperior(user: UserInfo | null | undefined): boolean {
        if (!user) {
            return false;
        }

        return user.isSuperior;
    }
}

type PolicyFunction = (scope: UserInfo | null | undefined) => boolean;

type AuthorizationPolicyPrototype = typeof AuthorizationPolicy;

type PolicyFunctions = {
    [P in keyof AuthorizationPolicyPrototype as AuthorizationPolicyPrototype[P] extends PolicyFunction
        ? P
        : never]: AuthorizationPolicyPrototype[P];
};

export type PolicyNames = keyof PolicyFunctions;
