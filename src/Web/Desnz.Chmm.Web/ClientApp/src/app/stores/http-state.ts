export interface HttpState<T = void> {
  loading: boolean;
  errorMessage: any;
  data: T | null;
}

export const defaultHttpState = <T = void>(): HttpState<T> => {
  return {
    loading: false,
    errorMessage: null,
    data: null,
  };
};
