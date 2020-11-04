export interface CreateEmailRequest {
    type: string,
    subject: string,
    body: string
};

export interface BadCreateEmailResponse {
    errors: object
};

export interface OkCreateEmailResponse {
    type: string,
    results: SendEmailResult[]
};

export interface SendEmailResult {
    emailAddress: string,
    isSuccess: boolean
};
