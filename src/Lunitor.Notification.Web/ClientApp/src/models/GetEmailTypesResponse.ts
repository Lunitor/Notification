export interface GetEmailTypesResponse {
    emailTypes: EmailTypeResponse[]
};

export interface EmailTypeResponse {
    name: string,
    placeholders: object
}