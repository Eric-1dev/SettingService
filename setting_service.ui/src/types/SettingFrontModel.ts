import { ApplicationFrontModel } from "./ApplicationFrontModel";

export interface SettingFrontModel {
    allApplications: ApplicationFrontModel[];
    settings: SettingItemFrontModel[];
}

export interface SettingItemFrontModel {
    key: number;
    name: string;
    description: string;
    value: string | null | undefined;
    valueType: string;
    isFromExternalSource: boolean;
    externalSourceType: string | null | undefined;
    externalSourcePath: string | null | undefined;
    externalSourceKey: string | null | undefined;
    applicationNames: string[];
}

export enum SettingValueTypeEnum {
    Int = 1,
    Long = 2,
    Double = 3,
    Decimal = 4,
    String = 5,
    Bool = 6,
}

export enum ExternalSourceTypeEnum {
    Vault = 1,
}
