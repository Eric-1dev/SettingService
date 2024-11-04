import { NotificationApiInjection } from "naive-ui/es/notification/src/NotificationProvider";

export const notify = (
    notification: NotificationApiInjection,
    notificationType: "info" | "success" | "warning" | "error",
    text: string
): void => {
    notification.create({
        type: notificationType,
        duration: 3000,
        keepAliveOnHover: true,
        content: text,
    });
};
