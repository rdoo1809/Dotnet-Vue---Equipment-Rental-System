<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>{{ pageTitle }}</h1>
      <button 
        v-if="isAdmin" 
        @click="showCreateModal = true" 
        class="btn btn-primary"
      >
        Add Equipment
      </button>
    </div>
    
    <div class="card">
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-striped">
            <thead>
              <tr>
                <th>Name</th>
                <th>Category</th>
                <th>Condition</th>
                <th>Status</th>
                <th v-if="isAdmin">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="equipment in equipmentList" :key="equipment.id">
                <td>{{ equipment.name }}</td>
                <td>{{ equipment.category }}</td>
                <td>{{ equipment.condition }}</td>
                <td>
                  <span :class="getStatusClass(equipment.status)">
                    {{ equipment.status }}
                  </span>
                </td>
                <td v-if="isAdmin">
                  <div class="btn-group" role="group">
                    <router-link 
                      :to="`/equipment/${equipment.id}`" 
                      class="btn btn-sm btn-outline-primary"
                    >
                      View
                    </router-link>
                    <button 
                      @click="editEquipment(equipment)" 
                      class="btn btn-sm btn-outline-secondary"
                    >
                      Edit
                    </button>
                    <button 
                      @click="deleteEquipment(equipment)" 
                      class="btn btn-sm btn-outline-danger"
                    >
                      Delete
                    </button>
                  </div>
                </td>
                <td v-else>
                  <router-link 
                    :to="`/equipment/${equipment.id}`" 
                    class="btn btn-sm btn-primary"
                  >
                    Details
                  </router-link>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
    
    <!-- Create/Edit Modal -->
    <EquipmentModal 
      v-if="showCreateModal || showEditModal"
      :equipment="editingEquipment"
      @close="closeModal"
      @save="handleSave"
    />
    
    <!-- Delete Confirmation Modal -->
    <ConfirmationModal
      v-if="showDeleteModal"
      title="Delete Equipment"
      message="Are you sure you want to delete this equipment?"
      @confirm="confirmDelete"
      @cancel="showDeleteModal = false"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { equipmentService } from '@/services/equipment'
import EquipmentModal from '@/components/EquipmentModal.vue'
import ConfirmationModal from '@/components/ConfirmationModal.vue'

const route = useRoute()
const authStore = useAuthStore()
const isAdmin = computed(() => authStore.user?.role === 'Admin')
const isAvailableView = computed(() => route.name === 'EquipmentAvailable')
const pageTitle = computed(() => isAvailableView.value ? 'Available Equipment' : 'Equipment')

const equipmentList = ref([])
const showCreateModal = ref(false)
const showEditModal = ref(false)
const showDeleteModal = ref(false)
const editingEquipment = ref(null)
const deletingEquipment = ref(null)

onMounted(() => {
  loadEquipment()
})

const loadEquipment = async () => {
  try {
    if (isAvailableView.value) {
      equipmentList.value = await equipmentService.getAvailable()
    } else {
      equipmentList.value = await equipmentService.getAll()
    }
  } catch (error) {
    console.error('Failed to load equipment:', error)
  }
}

const getStatusClass = (status) => {
  const classes = {
    'Available': 'badge bg-success',
    'Rented': 'badge bg-warning',
    'Maintenance': 'badge bg-danger'
  }
  return classes[status] || 'badge bg-secondary'
}

const editEquipment = (equipment) => {
  editingEquipment.value = equipment
  showEditModal.value = true
}

const deleteEquipment = (equipment) => {
  deletingEquipment.value = equipment
  showDeleteModal.value = true
}

const closeModal = () => {
  showCreateModal.value = false
  showEditModal.value = false
  editingEquipment.value = null
}

const handleSave = async (equipmentData) => {
  try {
    if (editingEquipment.value) {
      await equipmentService.update(editingEquipment.value.id, equipmentData)
    } else {
      await equipmentService.create(equipmentData)
    }
    closeModal()
    loadEquipment()
  } catch (error) {
    console.error('Failed to save equipment:', error)
  }
}

const confirmDelete = async () => {
  try {
    await equipmentService.delete(deletingEquipment.value.id)
    showDeleteModal.value = false
    deletingEquipment.value = null
    loadEquipment()
  } catch (error) {
    console.error('Failed to delete equipment:', error)
  }
}
</script>
