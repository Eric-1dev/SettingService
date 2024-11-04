import ApplicationsManagePage from "@/views/ApplicationsManagePage.vue";
import HomePage from "@/views/HomePage.vue";
import LoginPage from "@/views/LoginPage.vue";
import SettingsPage from "@/views/SettingsPage.vue";
import UsersPage from "@/views/UsersPage.vue";
import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";

const routes: Array<RouteRecordRaw> = [
    {
        path: "/",
        name: "home-page",
        meta: { pageName: "Домашняя страница" },
        component: HomePage,
    },
    {
        path: "/settings",
        name: "settings-page",
        meta: { pageName: "Настройки" },
        component: SettingsPage,
    },
    {
        path: "/applications",
        name: "applications-manage-page",
        meta: { pageName: "Приложения" },
        component: ApplicationsManagePage,
    },
    {
        path: "/users",
        name: "users-page",
        meta: { pageName: "Пользователи" },
        component: UsersPage,
    },
    {
        path: "/login",
        name: "login",
        meta: { pageName: "Авторизация" },
        component: LoginPage,
    },

    // {
    //   path: '/about',
    //   name: 'about',
    //   // route level code-splitting
    //   // this generates a separate chunk (about.[hash].js) for this route
    //   // which is lazy-loaded when the route is visited.
    //   component: () => import(/* webpackChunkName: "about" */ '../views/AboutView.vue')
    // }
];

const router = createRouter({
    history: createWebHistory(process.env.BASE_URL),
    routes,
});

export default router;
