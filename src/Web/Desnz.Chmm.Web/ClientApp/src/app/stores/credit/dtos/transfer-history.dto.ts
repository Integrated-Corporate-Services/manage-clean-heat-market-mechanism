export interface TransferHistoryDto {
  transfersIn: TransferHistoryInDto[];
  transfersOut: TransferHistoryOutDto[];
}

export interface TransferHistoryInDto {
  transferDateTime: string;
  organisationName: string;
  value: number;
}

export interface TransferHistoryOutDto {
  transferDateTime: string;
  organisationName: string;
  value: number;
  transferredBy: string;
}
