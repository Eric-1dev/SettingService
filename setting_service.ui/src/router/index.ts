import ApplicationsPage from '@/views/ApplicationsPage.vue'
import HomePage from '@/views/HomePage.vue'
import LoginPage from '@/views/LoginPage.vue'
import SettingsPage from '@/views/SettingsPage.vue'
import UsersPage from '@/views/UsersPage.vue'
import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'

const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    name: 'home-page',
    component: HomePage
  },
  {
    path: '/settings',
    name: 'settings-page',
    component: SettingsPage
  },
  {
    path: '/applications',
    name: 'applications-page',
    component: ApplicationsPage
  },
  {
    path: '/users',
    name: 'users-page',
    component: UsersPage
  },
  {
    path: '/login',
    name: 'login',
    component: LoginPage
  },

  // {
  //   path: '/about',
  //   name: 'about',
  //   // route level code-splitting
  //   // this generates a separate chunk (about.[hash].js) for this route
  //   // which is lazy-loaded when the route is visited.
  //   component: () => import(/* webpackChunkName: "about" */ '../views/AboutView.vue')
  // }
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

export default router
