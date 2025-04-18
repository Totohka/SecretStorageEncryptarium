export const EntityEnum = {
    None: 0,
    ApiKey: 1,
    Audit: 2,
    RefreshToken: 3,
    Role: 4,
    RoleType: 5,
    Secret: 6,
    SecretLinkPolicy: 7,
    SecretPolicy: 8,
    Storage: 9,
    StorageLinkPolicy: 10,
    StoragePolicy: 11,
    User: 12,
    UserRole: 13,
    WhiteListIp: 14,
    All: -1,
}

export const TypeStorageEnum = {
    None: 0,
    Personal: 1,
    Group: 2,
    Common: 3,
    All: -1,
}

export const AuthorizePoliciesEnum = {
    TokenPolicy: 0,
    StoragePolicy: 1,
    SecretPolicy: 2,
    UserPolicy: 3,
    None: 4,
    All: -1,
}

export const ControllersEnum = {
    RoleController: 0,
    RoleTypeController: 1,
    SecretPolicyController: 2,
    StoragePolicyController: 3,
    UserRightController: 4,
    ApiKeyController: 5,
    OAuthController: 6,
    RefreshTokenController: 7,
    TokenController: 8,
    UserPassController: 9,
    WhiteIpController: 10,
    AdminKeyController: 11,
    SecretController: 12,
    StorageController: 13,
    AuditController: 14,
    UserController: 15,
    All: -1,
}

export const MicroservicesEnum = {
    Access: 0,
    Auth: 1,
    Storage: 2,
    Audit: 3,
    All: -1,
}

export const PartHttpContextEnum = {
    None: 0,
    RequestBody: 1,
    RequestParameter: 2,
    ResponseBody: 3,
    All: -1,
}

export const StatusCodeEnum = {
    Success: 0,
    Error: 1,
    Warning: 2,
    All: -1,
}