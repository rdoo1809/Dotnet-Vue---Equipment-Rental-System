<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>Customers</h1>
      <button @click="showCreateModal = true" class="btn btn-primary">
        Add Customer
      </button>
    </div>
    
    <div class="card">
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-striped">
            <thead>
              <tr>
                <th>Name</th>
                <th>Username</th>
                <th>Role</th>
                <th>Active Rental</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="customer in customers" :key="customer.id">
                <td>{{ customer.name }}</td>
                <td>{{ customer.username }}</td>
                <td>
                  <span :class="getRoleClass(customer.role)">
                    {{ customer.role }}
                  </span>
                </td>
                <td>
                  <span v-if="customer.activeRental" class="text-warning">
                    {{ customer.activeRental.equipmentName }}
                  </span>
                  <span v-else class="text-muted">None</span>
                </td>
                <td>
                  <div class="btn-group" role="group">
                    <router-link 
                      :to="`/customer/${customer.id}`" 
                      class="btn btn-sm btn-outline-primary"
                    >
                      View
                    </router-link>
                    <button 
                      @click="editCustomer(customer)" 
                      class="btn btn-sm btn-outline-secondary"
                    >
                      Edit
                    </button>
                    <button 
                      @click="deleteCustomer(customer)" 
                      class="btn btn-sm btn-outline-danger"
                    >
                      Delete
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
    
    <!-- Create/Edit Modal -->
    <CustomerModal 
      v-if="showCreateModal || showEditModal"
      :customer="editingCustomer"
      @close="closeModal"
      @save="handleSave"
    />
    
    <!-- Delete Confirmation Modal -->
    <ConfirmationModal
      v-if="showDeleteModal"
      title="Delete Customer"
      message="Are you sure you want to delete this customer and their history?"
      @confirm="confirmDelete"
      @cancel="showDeleteModal = false"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { customerService } from '@/services/customer'
import CustomerModal from '@/components/CustomerModal.vue'
import ConfirmationModal from '@/components/ConfirmationModal.vue'

const customers = ref([])
const showCreateModal = ref(false)
const showEditModal = ref(false)
const showDeleteModal = ref(false)
const editingCustomer = ref(null)
const deletingCustomer = ref(null)

onMounted(() => {
  loadCustomers()
})

const loadCustomers = async () => {
  try {
    customers.value = await customerService.getAll()
  } catch (error) {
    console.error('Failed to load customers:', error)
  }
}

const getRoleClass = (role) => {
  return role === 'Admin' ? 'badge bg-danger' : 'badge bg-primary'
}

const editCustomer = (customer) => {
  editingCustomer.value = customer
  showEditModal.value = true
}

const deleteCustomer = (customer) => {
  deletingCustomer.value = customer
  showDeleteModal.value = true
}

const closeModal = () => {
  showCreateModal.value = false
  showEditModal.value = false
  editingCustomer.value = null
}

const handleSave = async (customerData) => {
  try {
    if (editingCustomer.value) {
      await customerService.update(editingCustomer.value.id, customerData)
    } else {
      await customerService.create(customerData)
    }
    closeModal()
    loadCustomers()
  } catch (error) {
    console.error('Failed to save customer:', error)
  }
}

const confirmDelete = async () => {
  try {
    await customerService.delete(deletingCustomer.value.id)
    showDeleteModal.value = false
    deletingCustomer.value = null
    loadCustomers()
  } catch (error) {
    console.error('Failed to delete customer:', error)
  }
}
</script>
