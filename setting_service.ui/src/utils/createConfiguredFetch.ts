import { createFetch } from "@vueuse/core";
import { MessageReactive } from "naive-ui";
import { MessageApiInjection } from "naive-ui/es/message/src/MessageProvider";

export const createConfiguredFetch = (
    message: MessageApiInjection,
    controllerUrl: string
) => {
    let loadingMessage: MessageReactive;

    const ssFetch = createFetch({
        baseUrl: `${process.env.VUE_APP_API_BASE_URL}/${controllerUrl}`,
        options: {
            async beforeFetch({ options }) {
                options.headers = {
                    ...options.headers,
                    Authorization: `Bearer qwerty123`,
                };

                loadingMessage = message.create("Выполняется запрос", {
                    type: "loading",
                    duration: 0,
                });
            },
            async afterFetch(ctx) {
                if (ctx.data?.isSuccess === false) {
                    // если вернулся код 200, но ошибка бизнесовая, считаем ответ невалидным
                    throw new Error(ctx.data?.message);
                } else {
                    loadingMessage.destroy();
                }

                return ctx;
            },
            async onFetchError(ctx) {
                loadingMessage.type = "error";
                if (ctx.response?.status) {
                    if (
                        ctx.response.status >= 200 &&
                        ctx.response.status <= 299
                    ) {
                        loadingMessage.content = ctx.data?.message;
                    } else if (
                        (ctx.response.status >= 402 &&
                            ctx.response.status <= 499) ||
                        ctx.response.status === 400
                    ) {
                        loadingMessage.content = "Некорректный запрос";
                    } else if (ctx.response.status === 401) {
                        loadingMessage.content = "Ошибка авторизации";
                    } else if (
                        ctx.response.status >= 500 &&
                        ctx.response.status <= 599
                    ) {
                        loadingMessage.content = "Ошибка при вызове API";
                    }
                } else {
                    loadingMessage.content = ctx.error.message;
                }

                setTimeout(() => {
                    loadingMessage.destroy();
                }, 3000);

                return ctx;
            },
        },
        fetchOptions: {
            mode: "cors",
        },
    });

    return ssFetch;
};
