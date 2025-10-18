import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const routes = [
  {
    path: '/',
    redirect: '/dashboard'
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/Login.vue'),
    meta: { requiresGuest: true }
  },
  {
    path: '/dashboard',
    name: 'Dashboard',
    component: () => import('@/views/Dashboard.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/equipment',
    name: 'Equipment',
    component: () => import('@/views/Equipment.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/equipment/:id',
    name: 'EquipmentDetails',
    component: () => import('@/views/EquipmentDetails.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/customer',
    name: 'Customers',
    component: () => import('@/views/Customers.vue'),
    meta: { requiresAuth: true, requiresAdmin: true }
  },
  {
    path: '/customer/:id',
    name: 'CustomerDetails',
    component: () => import('@/views/CustomerDetails.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/rental',
    name: 'Rentals',
    component: () => import('@/views/Rentals.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/rental/:id',
    name: 'RentalDetails',
    component: () => import('@/views/RentalDetails.vue'),
    meta: { requiresAuth: true }
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore()
  
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login')
    return
  }
  
  if (to.meta.requiresGuest && authStore.isAuthenticated) {
    next('/dashboard')
    return
  }
  
  if (to.meta.requiresAdmin && authStore.user?.role !== 'Admin') {
    next('/dashboard')
    return
  }
  
  next()
})

export default router
