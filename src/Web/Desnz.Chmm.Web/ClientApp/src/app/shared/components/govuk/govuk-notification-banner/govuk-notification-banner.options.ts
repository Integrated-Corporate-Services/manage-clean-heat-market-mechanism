export interface GovukNotificationBanner {
  title: string;
  text: string;
  link: GovukNotificationBannerLink | null;
}

export interface GovukNotificationBannerLink {
  text: string;
  url: string;
}
